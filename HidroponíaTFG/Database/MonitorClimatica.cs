using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Maui.Dispatching;

namespace HidroponíaTFG.Database
{
    public class MonitorClimatica
    {
        private readonly IMongoCollection<BsonDocument> _climaticaCollection;
        private readonly IMongoCollection<BsonDocument> _climaticaOptCollection;
        private readonly IMongoCollection<BsonDocument> _climaticaActCollection;
        private readonly IMongoCollection<BsonDocument> _registroCollection;
        private readonly CancellationTokenSource _cancellationTokenSource;
        private readonly ContentPage _page;

        public MonitorClimatica(ContentPage page)
        {
            var client = new MongoClient("mongodb+srv://root:root@proyectotfg.vprqszh.mongodb.net/?retryWrites=true&w=majority&appName=ProyectoTFG");
            var database = client.GetDatabase("ProyectoTFG");
            _climaticaCollection = database.GetCollection<BsonDocument>("Climatica");
            _climaticaOptCollection = database.GetCollection<BsonDocument>("Climatica_opt");
            _climaticaActCollection = database.GetCollection<BsonDocument>("Climatica_act");
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
                await CheckClimaticaStatus();
                await CheckClimaticaOptStatus();
                await CheckClimaticaActStatus();

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

        private async Task CheckClimaticaStatus()
        {
            var latestEntry = await _climaticaCollection.Find(new BsonDocument()).Sort("{_id: -1}").FirstOrDefaultAsync();

            if (latestEntry != null)
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    UpdateButtonText("ButonZona1", latestEntry["sensor1"].ToString() + "º");
                    UpdateButtonText("ButonZona2", latestEntry["sensor2"].ToString() + "º");
                    UpdateButtonText("ButonZona3", latestEntry["sensor3"].ToString() + "º");
                    UpdateButtonText("ButonZona4", latestEntry["sensor4"].ToString() + "º");
                    UpdateButtonText("ButonZona5", latestEntry["sensor5"].ToString() + "º");
                    UpdateButtonText("ButonZona6", latestEntry["sensor6"].ToString() + "º");
                    UpdateButtonText("ButonZona7", latestEntry["sensor7"].ToString() + "º");
                    UpdateButtonText("ButonZona8", latestEntry["sensor8"].ToString() + "º");
                });
            }
        }

        private async Task CheckClimaticaOptStatus()
        {
            var latestEntry = await _climaticaOptCollection.Find(new BsonDocument()).Sort("{_id: -1}").FirstOrDefaultAsync();

            if (latestEntry != null)
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    // Actualizar los Entry con los valores obtenidos
                    UpdateEntryText("entry1", latestEntry.GetValue("optimoTemperatura", "").ToString());
                    UpdateEntryText("entry2", latestEntry.GetValue("renovacionAire", "").ToString());
                });
            }
        }

        private async Task CheckClimaticaActStatus()
        {
            var latestEntry = await _climaticaActCollection.Find(new BsonDocument()).Sort("{_id: -1}").FirstOrDefaultAsync();

            if (latestEntry != null)
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    UpdateButtonText("acta1", latestEntry.GetValue("acta1", "").ToString());
                    UpdateButtonText("acta2", latestEntry.GetValue("acta2", "").ToString());

                    UpdateButtonText("actv1", latestEntry.GetValue("actv1", "").ToString());
                    UpdateButtonText("actv2", latestEntry.GetValue("actv2", "").ToString());
                    UpdateButtonText("actv3", latestEntry.GetValue("actv3", "").ToString());
                    UpdateButtonText("actv4", latestEntry.GetValue("actv4", "").ToString());

                    UpdateButtonText("actr1", latestEntry.GetValue("actr1", "").ToString());
                    UpdateButtonText("actr2", latestEntry.GetValue("actr2", "").ToString());
                    UpdateButtonText("actr3", latestEntry.GetValue("actr3", "").ToString());
                    UpdateButtonText("actr4", latestEntry.GetValue("actr4", "").ToString());
                    UpdateButtonText("actr5", latestEntry.GetValue("actr5", "").ToString());
                    UpdateButtonText("actr6", latestEntry.GetValue("actr6", "").ToString());
                    UpdateButtonText("actr7", latestEntry.GetValue("actr7", "").ToString());
                    UpdateButtonText("actr8", latestEntry.GetValue("actr8", "").ToString());
                });
            }
        }

        private void UpdateButtonText(string buttonName, string text)
        {
            var button = _page.FindByName<Button>(buttonName);
            if (button != null)
            {
                if (text == "false")
                {
                    button.BackgroundColor = Microsoft.Maui.Graphics.Colors.Grey;
                }
                else if (text == "true")
                {
                    button.BackgroundColor = Microsoft.Maui.Graphics.Colors.Green;
                }
                else
                {
                    button.Text = text;
                }
                
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

        public async Task SaveClimaticaData(string optimoTemperatura, string renovacionAire, string usuario)
        {
            if (string.IsNullOrEmpty(optimoTemperatura) || string.IsNullOrEmpty(renovacionAire) || string.IsNullOrEmpty(usuario))
            {
                throw new ArgumentException("Los parámetros no pueden ser nulos o vacíos.");
            }

            var filter = new BsonDocument(); // Asume que solo hay un documento para actualizar
            var update = Builders<BsonDocument>.Update
                .Set("optimoTemperatura", optimoTemperatura)
                .Set("renovacionAire", renovacionAire);

            var options = new UpdateOptions { IsUpsert = true };

            // Realizar la actualización
            await _climaticaOptCollection.UpdateOneAsync(filter, update, options);

            // Crear y empezar el hilo para registrar cambios
            var registroCambioThread = new RegistroCambioThread(_registroCollection, _climaticaOptCollection, usuario, optimoTemperatura, renovacionAire);
            registroCambioThread.StartClimatica();
        }
    }
}
