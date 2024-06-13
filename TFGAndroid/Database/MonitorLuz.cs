using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TFGAndroid.Models;

namespace TFGAndroid.Database
{
    public class MonitorLuz
    {
        private readonly IMongoCollection<BsonDocument> _luzCollection;// Colección para datos de luminosidad
        private readonly IMongoCollection<BsonDocument> _luzOptCollection;// Colección para opciones de luminosidad
        private readonly IMongoCollection<BsonDocument> _registroCollection;// Colección para registro de cambios

        private readonly CancellationTokenSource _cancellationTokenSource;// Fuente de cancelación para detener el monitoreo
        private readonly ContentPage _page;// Página de la aplicación donde se actualizan los elementos de la interfaz de usuario

        // Constructor de la clase MonitorLuz
        public MonitorLuz(ContentPage page)
        {
            var client = new MongoClient("mongodb://root:root@ac-pn6khua-shard-00-00.vprqszh.mongodb.net:27017,ac-pn6khua-shard-00-01.vprqszh.mongodb.net:27017,ac-pn6khua-shard-00-02.vprqszh.mongodb.net:27017/?ssl=true&replicaSet=atlas-a8s0cb-shard-0&authSource=admin&retryWrites=true&w=majority&appName=ProyectoTFG");
            var database = client.GetDatabase("ProyectoTFG");
            // Obtener las colecciones de MongoDB necesarias
            _luzCollection = database.GetCollection<BsonDocument>("Luminica");
            _luzOptCollection = database.GetCollection<BsonDocument>("Luminica_opt");
            _registroCollection = database.GetCollection<BsonDocument>("Registro");


            _cancellationTokenSource = new CancellationTokenSource();
            _page = page;
        }

        // Método para iniciar el monitoreo continuo de los datos de luminosidad y opciones
        public void StartMonitoring()
        {
            Task.Run(async () => await MonitorLoop(_cancellationTokenSource.Token));
        }

        // Método para detener el monitoreo de los datos de luminosidad y opciones
        public void StopMonitoring()
        {
            _cancellationTokenSource.Cancel();
        }

        // Bucle principal del monitoreo para los datos de luminosidad
        private async Task MonitorLoop(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                await CheckLuzStatus();// Verificar y actualizar el estado actual de los datos de luminosidad
                await LoadLuzOptStatus();// Cargar el estado actual de las opciones de luminosidad

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

        // Método para verificar y actualizar el estado actual de los datos de luminosidad
        private async Task CheckLuzStatus()
        {
            var latestEntry = await _luzCollection.Find(new BsonDocument()).Sort("{_id: -1}").FirstOrDefaultAsync();

            if (latestEntry != null)
            {
                // Actualizar botones de la interfaz de usuario con los valores obtenidos
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    UpdateButtonText("btn1", "Nivel de luz en linea 1: " + latestEntry["nivel1"].ToString());
                    UpdateButtonText("btn2", "Nivel de luz en linea 2: " + latestEntry["nivel2"].ToString());
                    UpdateButtonText("btn3", "Nivel de luz en linea 3: " + latestEntry["nivel3"].ToString());
                    UpdateButtonText("btn4", "Nivel de luz en linea 4: " + latestEntry["nivel4"].ToString());
                    UpdateButtonText("btn5", "Nivel de luz en linea 5: " + latestEntry["nivel5"].ToString());
                    UpdateButtonText("btn6", "Nivel de luz en linea 6: " + latestEntry["nivel6"].ToString());
                    UpdateButtonText("btn7", "Nivel de luz en linea 7: " + latestEntry["nivel7"].ToString());
                    UpdateButtonText("btn8", "Nivel de luz en linea 8: " + latestEntry["nivel8"].ToString());

                    UpdateButtonText("btn11", "Potencia de luz en linea 1: " + latestEntry["potencia1"].ToString());
                    UpdateButtonText("btn22", "Potencia de luz en linea 2: " + latestEntry["potencia2"].ToString());
                    UpdateButtonText("btn33", "Potencia de luz en linea 3: " + latestEntry["potencia3"].ToString());
                    UpdateButtonText("btn44", "Potencia de luz en linea 4: " + latestEntry["potencia4"].ToString());
                    UpdateButtonText("btn55", "Potencia de luz en linea 5: " + latestEntry["potencia5"].ToString());
                    UpdateButtonText("btn66", "Potencia de luz en linea 6: " + latestEntry["potencia6"].ToString());
                    UpdateButtonText("btn77", "Potencia de luz en linea 7: " + latestEntry["potencia7"].ToString());
                    UpdateButtonText("btn88", "Potencia de luz en linea 8: " + latestEntry["potencia8"].ToString());
                });
            }
        }

        // Método para cargar y actualizar el estado actual de las opciones de luminosidad
        private async Task LoadLuzOptStatus()
        {
            var latestOptEntry = await _luzOptCollection.Find(new BsonDocument()).Sort("{_id: -1}").FirstOrDefaultAsync();

            if (latestOptEntry != null)
            {
                // Actualizar entradas de la interfaz de usuario con los valores obtenidos
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

        // Método para actualizar el estado de las opciones de luminosidad en la base de datos
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
