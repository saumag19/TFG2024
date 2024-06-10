using Microsoft.Maui.Controls;
using Microsoft.Maui.Dispatching;
using MongoDB.Bson;
using MongoDB.Driver;
using SkiaSharp;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace HidroponíaTFG.Database
{
    public class MonitorRed
    {
        private readonly IMongoCollection<BsonDocument> _redCollection;
        private readonly CancellationTokenSource _cancellationTokenSource;
        private readonly ContentPage _page;

        public MonitorRed(ContentPage page)
        {
            var client = new MongoClient("mongodb+srv://root:root@proyectotfg.vprqszh.mongodb.net/?retryWrites=true&w=majority&appName=ProyectoTFG");
            var database = client.GetDatabase("ProyectoTFG");
            _redCollection = database.GetCollection<BsonDocument>("Red");

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
                await CheckRedStatus();

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

        private async Task CheckRedStatus()
        {
            var latestEntry = await _redCollection.Find(new BsonDocument()).Sort("{_id: -1}").FirstOrDefaultAsync();

            if (latestEntry != null)
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    UpdateFrameColor("placa1", latestEntry["placa1"].AsBoolean);
                    UpdateFrameColor("placa2", latestEntry["placa2"].AsBoolean);
                    UpdateFrameColor("placa3", latestEntry["placa3"].AsBoolean);
                    UpdateFrameColor("placa4", latestEntry["placa4"].AsBoolean);
                    UpdateFrameColor("placa5", latestEntry["placa5"].AsBoolean);
                    UpdateFrameColor("placa6", latestEntry["placa6"].AsBoolean);
                    UpdateFrameColor("placa7", latestEntry["placa7"].AsBoolean);
                    UpdateFrameColor("placa8", latestEntry["placa8"].AsBoolean);
                });
            }
        }

        private void UpdateFrameColor(string placaName, bool status)
        {
            var frame = _page.FindByName<Frame>($"frame{placaName}");
            if (frame != null)
            {
                frame.BackgroundColor = status ? Microsoft.Maui.Graphics.Colors.LightSkyBlue : Microsoft.Maui.Graphics.Colors.Salmon;
            }
        }
    }
}
