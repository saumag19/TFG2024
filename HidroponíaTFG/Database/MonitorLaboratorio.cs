﻿using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using Microcharts;
using SkiaSharp;
using Microcharts.Maui;

namespace HidroponíaTFG.Database
{
    public class MonitorLaboratorio
    {
        // Colecciones de MongoDB
        private readonly IMongoCollection<BsonDocument> _laboratorioOptCollection;
        private readonly IMongoCollection<BsonDocument> _registroCollection;
        private  IMongoCollection<BsonDocument> _laboratorioCollection;
        // Fuente de token para cancelar las tareas asíncronas
        private readonly CancellationTokenSource _cancellationTokenSource;
        // Página de contenido
        private readonly ContentPage _page;
        // Grafico que se va a actualizar
        private readonly ChartView _chartView;
        private IMongoDatabase database;

        // Constructor que inicializa las colecciones de MongoDB y los componentes de la interfaz
        public MonitorLaboratorio(ContentPage page, ChartView chartView)
        {
            var client = new MongoClient("mongodb+srv://root:root@proyectotfg.vprqszh.mongodb.net/?retryWrites=true&w=majority&appName=ProyectoTFG");
            database = client.GetDatabase("ProyectoTFG");
            _laboratorioOptCollection = database.GetCollection<BsonDocument>("Laboratorio_opt");
            _laboratorioCollection = database.GetCollection<BsonDocument>("Laboratorio_p1");
            _registroCollection = database.GetCollection<BsonDocument>("Registro");

            _cancellationTokenSource = new CancellationTokenSource();
            _page = page;
            _chartView = chartView;
        }

        // Método para iniciar el monitoreo
        public void StartMonitoring()
        {
            Task.Run(async () => await MonitorLoop(_cancellationTokenSource.Token));
            Task.Run(async () => await MonitorLoop2(_cancellationTokenSource.Token));
        }

        // Método para detener el monitoreo
        public void StopMonitoring()
        {
            _cancellationTokenSource.Cancel();
        }

        // Bucle principal de monitoreo para cargar el estado óptimo del laboratorio
        private async Task MonitorLoop(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                await LoadLaboratorioOptStatus();

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

        // Bucle principal de monitoreo para cargar el estado del laboratorio p1
        private async Task MonitorLoop2(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                await LoadLaboratorioP1Status();

                // Esperar 1 minuto antes de la siguiente comprobación
                try
                {
                    await Task.Delay(TimeSpan.FromMinutes(0.01), cancellationToken);
                }
                catch (TaskCanceledException)
                {
                    // Ignorar la excepción si la tarea fue cancelada
                }
            }
        }

        // Método para cargar el estado óptimo del laboratorio
        private async Task LoadLaboratorioOptStatus()
        {
            var latestOptEntry = await _laboratorioOptCollection.Find(new BsonDocument()).Sort("{_id: -1}").FirstOrDefaultAsync();

            if (latestOptEntry != null)
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    UpdateEntryText("entry1", latestOptEntry["humedad_semilla"].ToString());
                    UpdateEntryText("entry2", latestOptEntry["oxigeno_semilla"].ToString());
                    UpdateEntryText("entry3", latestOptEntry["luz_semilla"].ToString());
                    UpdateEntryText("entry4", latestOptEntry["nutrientes_semilla"].ToString());
                    UpdateEntryText("entry5", latestOptEntry["humedad_plantula"].ToString());
                    UpdateEntryText("entry6", latestOptEntry["oxigeno_plantula"].ToString());
                    UpdateEntryText("entry7", latestOptEntry["luz_plantula"].ToString());
                    UpdateEntryText("entry8", latestOptEntry["nutrientes_plantula"].ToString());
                });
            }
        }

        // Método para cargar el estado del laboratorio p1
        private async Task LoadLaboratorioP1Status()
        {
            var latestP1Entry = await _laboratorioCollection.Find(new BsonDocument()).Sort("{_id: -1}").FirstOrDefaultAsync();

            if (latestP1Entry != null)
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    UpdateLabelText("lblHumedad", $"Nivel de humedad: {latestP1Entry["humedad"]}");
                    UpdateLabelText("lblOxigeno", $"Nivel de oxigeno: {latestP1Entry["oxigeno"]}");
                    UpdateLabelText("lblLuz", $"Nivel de luz: {latestP1Entry["luz"]}");
                    UpdateLabelText("lblNutrientes", $"Nivel de nutrientes: {latestP1Entry["nutrientes"]}");

                    // Actualizar ChartView
                    UpdateChartView(latestP1Entry);
                });
            }
        }

        // Método para actualizar el texto de un Entry en la interfaz
        private void UpdateEntryText(string entryName, string text)
        {
            var entry = _page.FindByName<Entry>(entryName);
            if (entry != null)
            {
                entry.Text = text;
            }
        }

        // Método para actualizar el texto de un Label en la interfaz
        private void UpdateLabelText(string labelName, string text)
        {
            var label = _page.FindByName<Label>(labelName);
            if (label != null)
            {
                label.Text = text;
            }
        }

        // Método para actualizar las entradas del ChartView
        private void UpdateChartView(BsonDocument latestP1Entry)
        {
            // Crear una lista de entradas para el ChartView
            var entries = new List<ChartEntry>();

            // Agregar las entradas con los datos del documento más reciente
            entries.Add(new ChartEntry(Convert.ToSingle(latestP1Entry["humedad"]))
            {
                Label = "Humedad",
                ValueLabel = Convert.ToString(latestP1Entry["humedad"]),
                Color = SKColor.Parse("#64B5F6")
            });
            entries.Add(new ChartEntry(Convert.ToSingle(latestP1Entry["oxigeno"]))
            {
                Label = "Oxígeno",
                ValueLabel = Convert.ToString(latestP1Entry["oxigeno"]),
                Color = SKColor.Parse("#FFA726")
            });
            entries.Add(new ChartEntry(Convert.ToSingle(latestP1Entry["luz"]))
            {
                Label = "Luz",
                ValueLabel = Convert.ToString(latestP1Entry["luz"]),
                Color = SKColor.Parse("#4CAF50")
            });
            entries.Add(new ChartEntry(Convert.ToSingle(latestP1Entry["nutrientes"]))
            {
                Label = "Nutrientes",
                ValueLabel = Convert.ToString(latestP1Entry["nutrientes"]),
                Color = SKColor.Parse("#9C27B0")
            });

            // Actualizar las entradas del ChartView
            _chartView.Chart = new HalfRadialGaugeChart { Entries = entries,
                AnimationProgress = 0,
                AnimationDuration = TimeSpan.Zero,
                IsAnimated = false
            };
        }

        // Métodos para actualizar los datos en la base de datos
        public async Task UpdateLaboratorioOpt(string field, string newValue, string usuario)
        {
            if (double.TryParse(newValue, out double numericValue))
            {
                var filter = Builders<BsonDocument>.Filter.Empty;
                var update = Builders<BsonDocument>.Update.Set(field, numericValue);

                // Obtener el valor anterior antes de actualizar
                var latestOptEntry = await _laboratorioOptCollection.Find(new BsonDocument()).Sort("{_id: -1}").FirstOrDefaultAsync();

                string oldValue = "";
                if (latestOptEntry != null && latestOptEntry.Contains(field))
                {
                    // Verificar el tipo de dato antes de convertirlo
                    if (latestOptEntry[field].BsonType == BsonType.Double)
                    {
                        oldValue = latestOptEntry[field].AsDouble.ToString(); // Convertir a string si es Double
                    }
                    else if (latestOptEntry[field].BsonType == BsonType.String)
                    {
                        oldValue = latestOptEntry[field].AsString; // Tomar directamente si es String
                    }
                    else
                    {
                        // Manejar otros tipos según sea necesario
                        oldValue = latestOptEntry[field].ToString(); // Convertir a string en caso general
                    }
                }


                await _laboratorioOptCollection.UpdateOneAsync(filter, update, new UpdateOptions { IsUpsert = true });

                // Crear y empezar el hilo para registrar cambios
                var registroCambioThreadLaboratorio = new RegistroCambioThreadLaboratorio(_registroCollection, usuario, field, oldValue, newValue);
                registroCambioThreadLaboratorio.Start();
            }
            else
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    _page.DisplayAlert("Error", "Solo se deben introducir números.", "OK");
                });
            }
        }

        // Método para cambiar la colección en base al nombre del botón
        public async Task ChangeCollection(string nombreBTN)
        {
            if(nombreBTN == "btn1s")
            {
                _laboratorioCollection = database.GetCollection<BsonDocument>("Laboratorio_s1");
            }
            else if (nombreBTN == "btn2s")
            {
                _laboratorioCollection = database.GetCollection<BsonDocument>("Laboratorio_s2");
            }
            else if (nombreBTN == "btn3s")
            {
                _laboratorioCollection = database.GetCollection<BsonDocument>("Laboratorio_s3");
            }
            else if (nombreBTN == "btn4s")
            {
                _laboratorioCollection = database.GetCollection<BsonDocument>("Laboratorio_s4");
            }
            else if (nombreBTN == "btn5s")
            {
                _laboratorioCollection = database.GetCollection<BsonDocument>("Laboratorio_s5");
            }
            else if (nombreBTN == "btn6s")
            {
                _laboratorioCollection = database.GetCollection<BsonDocument>("Laboratorio_s6");
            }
            else if (nombreBTN == "btn1p")
            {
                _laboratorioCollection = database.GetCollection<BsonDocument>("Laboratorio_p1");
            }
            else if (nombreBTN == "btn2p")
            {
                _laboratorioCollection = database.GetCollection<BsonDocument>("Laboratorio_p2");
            }
            else if (nombreBTN == "btn3p")
            {
                _laboratorioCollection = database.GetCollection<BsonDocument>("Laboratorio_p3");
            }
            else if (nombreBTN == "btn4p")
            {
                _laboratorioCollection = database.GetCollection<BsonDocument>("Laboratorio_p4");
            }
            else if (nombreBTN == "btn5p")
            {
                _laboratorioCollection = database.GetCollection<BsonDocument>("Laboratorio_p5");
            }
            else if (nombreBTN == "btn6p")
            {
                _laboratorioCollection = database.GetCollection<BsonDocument>("Laboratorio_p6");
            }

        }
    }
}
