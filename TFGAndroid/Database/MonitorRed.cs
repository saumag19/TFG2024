using Microsoft.Maui.Controls;
using Microsoft.Maui.Dispatching;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace TFGAndroid.Database
{
    public class MonitorRed
    {
        private readonly IMongoCollection<BsonDocument> _redCollection;// Colección para datos de red
        private readonly CancellationTokenSource _cancellationTokenSource; // Fuente de cancelación para detener el monitoreo
        private readonly ContentPage _page; // Página de la aplicación donde se actualizan los elementos de la interfaz de usuario

        // Constructor de la clase MonitorRed
        public MonitorRed(ContentPage page)
        {
            var client = new MongoClient("mongodb://root:root@ac-pn6khua-shard-00-00.vprqszh.mongodb.net:27017,ac-pn6khua-shard-00-01.vprqszh.mongodb.net:27017,ac-pn6khua-shard-00-02.vprqszh.mongodb.net:27017/?ssl=true&replicaSet=atlas-a8s0cb-shard-0&authSource=admin&retryWrites=true&w=majority&appName=ProyectoTFG");
            var database = client.GetDatabase("ProyectoTFG");
            // Obtener la colección de MongoDB necesaria
            _redCollection = database.GetCollection<BsonDocument>("Red");

            _cancellationTokenSource = new CancellationTokenSource();
            _page = page;
        }

        // Método para iniciar el monitoreo continuo del estado de red
        public void StartMonitoring()
        {
            Task.Run(async () => await MonitorLoop(_cancellationTokenSource.Token));
        }

        // Método para detener el monitoreo del estado de red
        public void StopMonitoring()
        {
            _cancellationTokenSource.Cancel();
        }

        // Bucle principal del monitoreo para el estado de red
        private async Task MonitorLoop(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                await CheckRedStatus();// Verificar y actualizar el estado actual de los datos de red

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

        // Método para verificar y actualizar el estado actual de los datos de red
        private async Task CheckRedStatus()
        {
            var latestEntry = await _redCollection.Find(new BsonDocument()).Sort("{_id: -1}").FirstOrDefaultAsync();

            if (latestEntry != null)
            {
                // Actualizar botones de la interfaz de usuario con los valores obtenidos
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    UpdateTextButton("placa1", latestEntry["placa1"].AsBoolean);
                    UpdateTextButton("placa2", latestEntry["placa2"].AsBoolean);
                    UpdateTextButton("placa3", latestEntry["placa3"].AsBoolean);
                    UpdateTextButton("placa4", latestEntry["placa4"].AsBoolean);
                    UpdateTextButton("placa5", latestEntry["placa5"].AsBoolean);
                    UpdateTextButton("placa6", latestEntry["placa6"].AsBoolean);
                    UpdateTextButton("placa7", latestEntry["placa7"].AsBoolean);
                    UpdateTextButton("placa8", latestEntry["placa8"].AsBoolean);
                });
            }
        }

        // Método para actualizar el estado de un botón en la interfaz de usuario según el estado de red
        private void UpdateTextButton(string placaName, bool status)
        {
            var frame = _page.FindByName<Button>($"frame{placaName}");
            if (frame != null)
            {
                frame.Text = status ? "ON" : "OFF";// Actualizar texto del botón según el estado de la placa
            }
        }
    }
}
