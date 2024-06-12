using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using Microcharts.Maui;
using Microcharts;
using SkiaSharp;

namespace HidroponíaTFG.Database
{
    public class MonitorHidraulica
    {
        private readonly IMongoCollection<BsonDocument> _hidraulicaCollection;
        private readonly IMongoCollection<BsonDocument> _hidraulicaOptCollection;
        private readonly IMongoCollection<BsonDocument> _registroCollection;
        private readonly CancellationTokenSource _cancellationTokenSource;
        private readonly ContentPage _page;
        private readonly ChartView _chartView1;
        private readonly ChartView _chartView2;
        private readonly ChartView _chartView3;
        private readonly ChartView _chartView5;
        private readonly ChartView _chartView6;

        public MonitorHidraulica(ContentPage page, ChartView chartView1, ChartView chartView2, ChartView chartView3, ChartView chartView5, ChartView chartView6)
        {
            var client = new MongoClient("mongodb+srv://root:root@proyectotfg.vprqszh.mongodb.net/?retryWrites=true&w=majority&appName=ProyectoTFG");
            var database = client.GetDatabase("ProyectoTFG");
            _hidraulicaCollection = database.GetCollection<BsonDocument>("Hidraulica");
            _hidraulicaOptCollection = database.GetCollection<BsonDocument>("Hidraulica_opt");
            _registroCollection = database.GetCollection<BsonDocument>("Registro");

            _cancellationTokenSource = new CancellationTokenSource();
            _page = page;
            _chartView1 = chartView1;
            _chartView2 = chartView2;
            _chartView3 = chartView3;
            _chartView5 = chartView5;
            _chartView6 = chartView6;
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
                    UpdateChartView();
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



        private async Task UpdateChartView()
        {
            var latestEntries = await _hidraulicaCollection.Find(new BsonDocument()).SortByDescending(x => x["_id"]).Limit(5).ToListAsync();

            if (latestEntries.Any())
            {
                var entries1 = latestEntries.Select(entry => new ChartEntry(Convert.ToSingle(entry["nivel_deposito"]))
                {
                    
                    ValueLabel = Convert.ToString(entry["nivel_deposito"]),
                    Color = SKColor.Parse("#64B5F6")
                }).ToList();

                _chartView1.Chart = new BarChart
                {
                    Entries = entries1,
                    AnimationProgress = 0,
                    AnimationDuration = TimeSpan.Zero,
                    IsAnimated = false
                };

                var entries2 = latestEntries.Select(entry => new ChartEntry(Convert.ToSingle(entry["oxigeno"]))
                {
                    
                    ValueLabel = Convert.ToString(entry["nitrogeno"]),
                    Color = SKColor.Parse("#FFA726")
                }).ToList();

                _chartView2.Chart = new LineChart
                {
                    Entries = entries2,
                    AnimationProgress = 0,
                    AnimationDuration = TimeSpan.Zero,
                    IsAnimated = false
                };

                var entries3 = latestEntries.Select(entry => new ChartEntry(Convert.ToSingle(entry["potasio"]))
                {
                    
                    ValueLabel = Convert.ToString(entry["potasio"]),
                    Color = SKColor.Parse("#B384C4")
                }).ToList();

                _chartView3.Chart = new LineChart
                {
                    Entries = entries3,
                    AnimationProgress = 0,
                    AnimationDuration = TimeSpan.Zero,
                    IsAnimated = false
                };

                var entries5 = latestEntries.Select(entry => new ChartEntry(Convert.ToSingle(entry["nitrogeno"]))
                {
                    
                    ValueLabel = Convert.ToString(entry["fosforo"]),
                    Color = SKColor.Parse("#AEBD4A")
                }).ToList();

                _chartView5.Chart = new LineChart
                {
                    Entries = entries5,
                    AnimationProgress = 0,
                    AnimationDuration = TimeSpan.Zero,
                    IsAnimated = false
                };

                var entries6 = latestEntries.Select(entry => new ChartEntry(Convert.ToSingle(entry["fosforo"]))
                {
                    
                    ValueLabel = Convert.ToString(entry["oxigeno"]),
                    Color = SKColor.Parse("#7EDFDC")
                }).ToList();

                _chartView6.Chart = new LineChart
                {
                    Entries = entries6,
                    AnimationProgress = 0,
                    AnimationDuration = TimeSpan.Zero,
                    IsAnimated = false
                };
            }


        }


    }
}
