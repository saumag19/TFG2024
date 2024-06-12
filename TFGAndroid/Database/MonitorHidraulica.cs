using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;

namespace TFGAndroid.Database
{
    public class MonitorHidraulica
    {
        private readonly IMongoCollection<BsonDocument> _hidraulicaCollection;
        private readonly IMongoCollection<BsonDocument> _hidraulicaOptCollection;
        private readonly IMongoCollection<BsonDocument> _registroCollection;
        private readonly CancellationTokenSource _cancellationTokenSource;
        private readonly ContentPage _page;

        public MonitorHidraulica(ContentPage page)
        {
            var client = new MongoClient("mongodb://root:root@ac-pn6khua-shard-00-00.vprqszh.mongodb.net:27017,ac-pn6khua-shard-00-01.vprqszh.mongodb.net:27017,ac-pn6khua-shard-00-02.vprqszh.mongodb.net:27017/?ssl=true&replicaSet=atlas-a8s0cb-shard-0&authSource=admin&retryWrites=true&w=majority&appName=ProyectoTFG");
            var database = client.GetDatabase("ProyectoTFG");
            _hidraulicaCollection = database.GetCollection<BsonDocument>("Hidraulica");
            _hidraulicaOptCollection = database.GetCollection<BsonDocument>("Hidraulica_opt");
            _registroCollection = database.GetCollection<BsonDocument>("Registro");


            _cancellationTokenSource = new CancellationTokenSource();
            _page = page;
        }

        public void StartMonitoring()
        {
            Task.Run(async () => await MonitorLoop(_cancellationTokenSource.Token));
        }

        public void StopMonitoring()
        {
            _cancellationTokenSource.Cancel();
        }

        private async Task MonitorLoop(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                await CheckHidraulicaStatus();

                // Esperar 1 minuto antes de la siguiente comprobación
                try
                {
                    await Task.Delay(TimeSpan.FromMinutes(1), cancellationToken);
                }
                catch (TaskCanceledException)
                {
                    // Ignorar la excepción si la tarea fue cancelada
                }
            }
        }

        private async Task CheckHidraulicaStatus()
        {
            var latestEntry = await _hidraulicaCollection.Find(new BsonDocument()).Sort("{_id: -1}").FirstOrDefaultAsync();
            var latestOptEntry = await _hidraulicaOptCollection.Find(new BsonDocument()).Sort("{_id: -1}").FirstOrDefaultAsync();

            if (latestEntry != null)
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    UpdateLabelText("lblNivelDeposito", "Nivel actual del depósito: " + latestEntry["nivel_deposito"].ToString());
                    UpdateLabelText("lblOxigeno", "Nivel actual de oxígeno: " + latestEntry["oxigeno"].ToString());
                    UpdateLabelText("lblPotasio", "Nivel actual de potasio: " + latestEntry["potasio"].ToString());
                    UpdateLabelText("lblNitrogeno", "Nivel actual de nitrógeno: " + latestEntry["nitrogeno"].ToString());
                    UpdateLabelText("lblFosforo", "Nivel actual de fósforo: " + latestEntry["fosforo"].ToString());
                });
            }

            if (latestOptEntry != null)
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    UpdateEntryText("entryNivelDeposito", latestOptEntry["nivel_deposito"].ToString());
                    UpdateEntryText("entryOxigeno", latestOptEntry["oxigeno"].ToString());
                    UpdateEntryText("entryPotasio", latestOptEntry["potasio"].ToString());
                    UpdateEntryText("entryNitrogeno", latestOptEntry["nitrogeno"].ToString());
                    UpdateEntryText("entryFosforo", latestOptEntry["fosforo"].ToString());
                });
            }
        }

        private void UpdateLabelText(string labelName, string text)
        {
            var label = _page.FindByName<Label>(labelName);
            if (label != null)
            {
                label.Text = text;
            }
        }

        private void UpdateEntryText(string entryName, string text)
        {
            var entry = _page.FindByName<Entry>(entryName);
            if (entry != null)
            {
                entry.Text = text;
            }
        }

        public async Task UpdateHidraulicaOpt(string field, string newValue, string usuario)
        {
            if (double.TryParse(newValue, out double numericValue))
            {
                var filter = Builders<BsonDocument>.Filter.Empty;
                var update = Builders<BsonDocument>.Update.Set(field, numericValue);
                await _hidraulicaOptCollection.UpdateOneAsync(filter, update, new UpdateOptions { IsUpsert = true });

                // Crear y empezar el hilo para registrar cambios
                var registroCambioThreadHidraulica = new RegistroCambioThreadHidraulica(_registroCollection, _hidraulicaOptCollection, usuario, field, newValue);
                registroCambioThreadHidraulica.Start();
            }
            else
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    _page.DisplayAlert("Error", "Solo se deben introducir números.", "OK");
                });
            }
        }


    }
}
