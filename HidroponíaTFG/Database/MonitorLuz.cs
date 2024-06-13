﻿using MongoDB.Bson;
using MongoDB.Driver;
using HidroponíaTFG.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HidroponíaTFG.Models;

namespace HidroponíaTFG.Database
{
    public class MonitorLuz
    {
        // Colecciones de MongoDB
        private readonly IMongoCollection<BsonDocument> _luzCollection;
        private readonly IMongoCollection<BsonDocument> _luzOptCollection;
        private readonly IMongoCollection<BsonDocument> _registroCollection;
        // Fuente de token para cancelar las tareas asíncronas
        private readonly CancellationTokenSource _cancellationTokenSource;
        // Página de contenido
        private readonly ContentPage _page;

        // Constructor de la clase MonitorLuz que inicializa las colecciones de MongoDB y otros campos necesarios
        public MonitorLuz(ContentPage page)
        {
            var client = new MongoClient("mongodb+srv://root:root@proyectotfg.vprqszh.mongodb.net/?retryWrites=true&w=majority&appName=ProyectoTFG");
            var database = client.GetDatabase("ProyectoTFG");
            _luzCollection = database.GetCollection<BsonDocument>("Luminica");
            _luzOptCollection = database.GetCollection<BsonDocument>("Luminica_opt");
            _registroCollection = database.GetCollection<BsonDocument>("Registro");

            _cancellationTokenSource = new CancellationTokenSource();
            _page = page;
        }

        // Método para iniciar el monitoreo de la luz en un bucle asíncrono
        public void StartMonitoring()
        {
            Task.Run(async () => await MonitorLoop(_cancellationTokenSource.Token));
        }

        // Método para detener el monitoreo de la luz
        public void StopMonitoring()
        {
            _cancellationTokenSource.Cancel();
        }

        // Bucle principal de monitoreo que verifica el estado de la luz y carga el estado óptimo de luz
        private async Task MonitorLoop(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                await CheckLuzStatus();
                await LoadLuzOptStatus();

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

        // Método para verificar el estado actual de la luz y actualizar la interfaz de usuario
        private async Task CheckLuzStatus()
        {
            var latestEntry = await _luzCollection.Find(new BsonDocument()).Sort("{_id: -1}").FirstOrDefaultAsync();

            if (latestEntry != null)
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    UpdateButtonText("btn1", "Nivel de luz en linea 1: " + latestEntry["nivel1"].ToString() + "     Potencia de luz en linea 1: " + latestEntry["potencia1"].ToString());
                    UpdateButtonText("btn2", "Nivel de luz en linea 2: " + latestEntry["nivel2"].ToString() + "     Potencia de luz en linea 2: " + latestEntry["potencia2"].ToString());
                    UpdateButtonText("btn3", "Nivel de luz en linea 3: " + latestEntry["nivel3"].ToString() + "     Potencia de luz en linea 3: " + latestEntry["potencia3"].ToString());
                    UpdateButtonText("btn4", "Nivel de luz en linea 4: " + latestEntry["nivel4"].ToString() + "     Potencia de luz en linea 4: " + latestEntry["potencia4"].ToString());
                    UpdateButtonText("btn5", "Nivel de luz en linea 5: " + latestEntry["nivel5"].ToString() + "     Potencia de luz en linea 5: " + latestEntry["potencia5"].ToString());
                    UpdateButtonText("btn6", "Nivel de luz en linea 6: " + latestEntry["nivel6"].ToString() + "     Potencia de luz en linea 6: " + latestEntry["potencia6"].ToString());
                    UpdateButtonText("btn7", "Nivel de luz en linea 7: " + latestEntry["nivel7"].ToString() + "     Potencia de luz en linea 7: " + latestEntry["potencia7"].ToString());
                    UpdateButtonText("btn8", "Nivel de luz en linea 8: " + latestEntry["nivel8"].ToString() + "     Potencia de luz en linea 8: " + latestEntry["potencia8"].ToString());
                });
            }
        }

        // Método para cargar el estado óptimo de luz y actualizar la interfaz de usuario
        private async Task LoadLuzOptStatus()
        {
            var latestOptEntry = await _luzOptCollection.Find(new BsonDocument()).Sort("{_id: -1}").FirstOrDefaultAsync();

            if (latestOptEntry != null)
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    var entry1 = _page.FindByName<Entry>("entry1");
                    var entry2 = _page.FindByName<Entry>("entry2");
                    if (entry1 != null && entry2 != null)
                    {
                        entry1.Text = latestOptEntry["nivel"].ToString();
                        entry2.Text = latestOptEntry["potencia"].ToString();
                    }
                });
            }
        }

        // Método para actualizar el estado óptimo de luz en la base de datos y registrar los cambios

        public async Task UpdateLuzOptStatus(string nivel, string potencia, Usuario usuario)
        {
                var filter = Builders<BsonDocument>.Filter.Empty;
                var existingDocument = await _luzOptCollection.Find(filter).FirstOrDefaultAsync();

                if (existingDocument == null)
                {
                    var newDocument = new BsonDocument
            {
                { "nivel", nivel },
                { "potencia", potencia }
            };
                    await _luzOptCollection.InsertOneAsync(newDocument);
                }
                else
                {
                    var update = Builders<BsonDocument>.Update
                        .Set("nivel", nivel)
                        .Set("potencia", potencia);
                    await _luzOptCollection.UpdateOneAsync(filter, update);
                }

                // Crear y comenzar hilo para registrar cambios
                var registroCambioThread = new RegistroCambioThreadLuz(_registroCollection, _luzOptCollection, nivel, potencia, usuario.Nombre);
                registroCambioThread.Start();
        }


        // Método para actualizar el texto de un botón en la interfaz de usuario
        private void UpdateButtonText(string buttonName, string text)
        {
            var button = _page.FindByName<Button>(buttonName);
            if (button != null)
            {
                button.Text = text;
            }
        }
    }
}
