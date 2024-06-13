﻿using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;

namespace TFGAndroid.Database
{
    public class MonitorLaboratorio
    {
        private readonly IMongoCollection<BsonDocument> _laboratorioOptCollection;// Colección para opciones de laboratorio
        private readonly IMongoCollection<BsonDocument> _registroCollection;// Colección para registro de cambios

        private IMongoCollection<BsonDocument> _laboratorioCollection;// Colección para datos de laboratorio seleccionada dinámicamente
        private readonly CancellationTokenSource _cancellationTokenSource;// Fuente de cancelación para detener el monitoreo
        private readonly ContentPage _page;// Página de la aplicación donde se actualizan los elementos de la interfaz de usuario
        private IMongoDatabase database;// Base de datos MongoDB

        // Constructor de la clase MonitorLaboratorio
        public MonitorLaboratorio(ContentPage page)
        {
            var client = new MongoClient("mongodb://root:root@ac-pn6khua-shard-00-00.vprqszh.mongodb.net:27017,ac-pn6khua-shard-00-01.vprqszh.mongodb.net:27017,ac-pn6khua-shard-00-02.vprqszh.mongodb.net:27017/?ssl=true&replicaSet=atlas-a8s0cb-shard-0&authSource=admin&retryWrites=true&w=majority&appName=ProyectoTFG");
            database = client.GetDatabase("ProyectoTFG");
            // Obtener las colecciones de MongoDB necesarias
            _laboratorioOptCollection = database.GetCollection<BsonDocument>("Laboratorio_opt");
            _laboratorioCollection = database.GetCollection<BsonDocument>("Laboratorio_p1");
            _registroCollection = database.GetCollection<BsonDocument>("Registro");

            _cancellationTokenSource = new CancellationTokenSource();
            _page = page;
        }

        // Método para iniciar el monitoreo continuo de los datos de laboratorio
        public void StartMonitoring()
        {
            Task.Run(async () => await MonitorLoop(_cancellationTokenSource.Token));
            Task.Run(async () => await MonitorLoop2(_cancellationTokenSource.Token));
        }

        // Método para detener el monitoreo de los datos de laboratorio
        public void StopMonitoring()
        {
            _cancellationTokenSource.Cancel();
        }

        // Bucle 1 del monitoreo para las opciones de laboratorio
        private async Task MonitorLoop(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                await LoadLaboratorioOptStatus();// Cargar el estado actual de las opciones de laboratorio

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
        // Bucle 2 del monitoreo para los datos de laboratorio específicos
        private async Task MonitorLoop2(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                
                await LoadLaboratorioP1Status();

                // Esperar 0.01 minuto antes de la siguiente comprobación
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

        // Método para cargar y actualizar el estado actual de las opciones de laboratorio
        private async Task LoadLaboratorioOptStatus()
        {
            var latestOptEntry = await _laboratorioOptCollection.Find(new BsonDocument()).Sort("{_id: -1}").FirstOrDefaultAsync();

            if (latestOptEntry != null)
            {
                // Actualizar entradas de la interfaz de usuario con los valores obtenidos
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

        // Método para cargar y actualizar el estado actual de los datos de laboratorio P1, la collection cambiará según el boton seleccionado
        private async Task LoadLaboratorioP1Status()
        {
            var latestP1Entry = await _laboratorioCollection.Find(new BsonDocument()).Sort("{_id: -1}").FirstOrDefaultAsync();

            if (latestP1Entry != null)
            {
                // Actualizar etiquetas de la interfaz de usuario con los valores obtenidos
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    UpdateLabelText("lblHumedad", $"Nivel de humedad: {latestP1Entry["humedad"]}");
                    UpdateLabelText("lblOxigeno", $"Nivel de oxigeno: {latestP1Entry["oxigeno"]}");
                    UpdateLabelText("lblLuz", $"Nivel de luz: {latestP1Entry["luz"]}");
                    UpdateLabelText("lblNutrientes", $"Nivel de nutrientes: {latestP1Entry["nutrientes"]}");
                });
            }
        }

        // Método para actualizar el texto de una entrada en la interfaz de usuario
        private void UpdateEntryText(string entryName, string text)
        {
            var entry = _page.FindByName<Entry>(entryName);
            if (entry != null)
            {
                entry.Text = text;
            }
        }

        // Método para actualizar el texto de una etiqueta en la interfaz de usuario
        private void UpdateLabelText(string labelName, string text)
        {
            var label = _page.FindByName<Label>(labelName);
            if (label != null)
            {
                label.Text = text;
            }
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

        // Método para cambiar dinámicamente la colección de laboratorio actual
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
