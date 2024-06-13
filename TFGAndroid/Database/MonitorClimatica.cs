using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Maui.Dispatching;

namespace TFGAndroid.Database
{
    public class MonitorClimatica
    {
        // Colecciones MongoDB para diferentes tipos de datos climáticos
        private readonly IMongoCollection<BsonDocument> _climaticaCollection;
        private readonly IMongoCollection<BsonDocument> _climaticaOptCollection;
        private readonly IMongoCollection<BsonDocument> _climaticaActCollection;
        private readonly IMongoCollection<BsonDocument> _registroCollection;
        private readonly CancellationTokenSource _cancellationTokenSource;// Fuente de cancelación para detener el monitoreo
        private readonly ContentPage _page; // Página de contenido asociada para actualizar la interfaz de usuario

        // Constructor que inicializa las colecciones y la fuente de cancelación
        public MonitorClimatica(ContentPage page)
        {
            // Configuración de la conexión MongoDB Atlas y obtención de las colecciones
            var client = new MongoClient("mongodb://root:root@ac-pn6khua-shard-00-00.vprqszh.mongodb.net:27017,ac-pn6khua-shard-00-01.vprqszh.mongodb.net:27017,ac-pn6khua-shard-00-02.vprqszh.mongodb.net:27017/?ssl=true&replicaSet=atlas-a8s0cb-shard-0&authSource=admin&retryWrites=true&w=majority&appName=ProyectoTFG");
            var database = client.GetDatabase("ProyectoTFG");
            _climaticaCollection = database.GetCollection<BsonDocument>("Climatica");
            _climaticaOptCollection = database.GetCollection<BsonDocument>("Climatica_opt");
            _climaticaActCollection = database.GetCollection<BsonDocument>("Climatica_act");
            _registroCollection = database.GetCollection<BsonDocument>("Registro");

            _cancellationTokenSource = new CancellationTokenSource();// Inicializa la fuente de cancelación
            _page = page;// Asigna la página de contenido proporcionada
        }

        // Método para iniciar el monitoreo climático
        public void StartMonitoring()
        {
            Task.Run(async () => await MonitorLoop(_cancellationTokenSource.Token));// Ejecuta el bucle de monitoreo en un hilo separado
        }

        // Método para detener el monitoreo climático
        public void StopMonitoring()
        {
            _cancellationTokenSource.Cancel();// Cancela la tarea de monitoreo al detener el token de cancelación
        }

        // Bucle de monitoreo que verifica periódicamente los datos climáticos
        private async Task MonitorLoop(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                await CheckClimaticaStatus();// Verifica el estado de los datos climáticos generales
                await CheckClimaticaOptStatus();// Verifica el estado de los datos climáticos óptimos
                await CheckClimaticaActStatus();// Verifica el estado de los datos climáticos activos

                // Espera 1 minuto antes de la siguiente comprobación, manejando excepciones de cancelación
                try
                {
                    await Task.Delay(TimeSpan.FromMinutes(1), cancellationToken);
                }
                catch (TaskCanceledException)
                {
                    // Ignora la excepción si la tarea fue cancelada
                }
            }
        }

        // Método para verificar el estado de los datos climáticos generales
        private async Task CheckClimaticaStatus()
        {
            var latestEntry = await _climaticaCollection.Find(new BsonDocument()).Sort("{_id: -1}").FirstOrDefaultAsync();

            if (latestEntry != null)
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    // Actualiza los botones de la interfaz con los datos climáticos obtenidos
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

        // Método para verificar el estado de los datos climáticos óptimos
        private async Task CheckClimaticaOptStatus()
        {
            var latestEntry = await _climaticaOptCollection.Find(new BsonDocument()).Sort("{_id: -1}").FirstOrDefaultAsync();

            if (latestEntry != null)
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    // Actualiza los Entry de la interfaz con los datos climáticos óptimos obtenidos
                    UpdateEntryText("entry1", latestEntry.GetValue("optimoTemperatura", "").ToString());
                    UpdateEntryText("entry2", latestEntry.GetValue("renovacionAire", "").ToString());
                });
            }
        }

        // Método para verificar el estado de los datos climáticos activos
        private async Task CheckClimaticaActStatus()
        {
            var latestEntry = await _climaticaActCollection.Find(new BsonDocument()).Sort("{_id: -1}").FirstOrDefaultAsync();

            if (latestEntry != null)
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    // Actualiza los botones de la interfaz con los datos climáticos activos obtenidos
                    UpdateButtonText("acta1", latestEntry.GetValue("acta1", "").ToString());
                    UpdateButtonText("acta2", latestEntry.GetValue("acta2", "").ToString());

                    UpdateButtonText("actr1", latestEntry.GetValue("actr1", "").ToString());
                    UpdateButtonText("actr2", latestEntry.GetValue("actr2", "").ToString());
                    UpdateButtonText("actr3", latestEntry.GetValue("actr3", "").ToString());
                    UpdateButtonText("actr4", latestEntry.GetValue("actr4", "").ToString());
                    UpdateButtonText("actr5", latestEntry.GetValue("actr5", "").ToString());
                    UpdateButtonText("actr6", latestEntry.GetValue("actr6", "").ToString());
                    UpdateButtonText("actr7", latestEntry.GetValue("actr7", "").ToString());
                    UpdateButtonText("actr8", latestEntry.GetValue("actr8", "").ToString());

                    UpdateButtonText("actv1", latestEntry.GetValue("actv1", "").ToString());
                    UpdateButtonText("actv2", latestEntry.GetValue("actv2", "").ToString());
                    UpdateButtonText("actv3", latestEntry.GetValue("actv3", "").ToString());
                    UpdateButtonText("actv4", latestEntry.GetValue("actv4", "").ToString());
                });
            }
        }

        // Método para actualizar el texto de los botones en la interfaz de usuario
        private void UpdateButtonText(string buttonName, string text)
        {
            var button = _page.FindByName<Button>(buttonName);
            if (button != null)
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    // Actualiza el texto del botón según los valores recibidos
                    if (text == "false")
                    {
                        button.Text = "OFF";
                    }
                    else if (text == "true")
                    {
                        button.Text = "ON";
                    }
                    else
                    {
                        button.Text = text;
                    }

                });
            }
        }



        // Método para actualizar el texto de los Entry en la interfaz de usuario
        private void UpdateEntryText(string entryName, string text)
        {
            var entry = _page.FindByName<Entry>(entryName);
            if (entry != null)
            {
                entry.Text = text;// Actualiza el texto del Entry con el valor recibido
            }
        }

        // Método para guardar los datos climáticos óptimos en la base de datos y registrar cambios
        public async Task SaveClimaticaData(string optimoTemperatura, string renovacionAire, string usuario)
        {
            // Verifica que los parámetros no sean nulos o vacíos
            if (string.IsNullOrEmpty(optimoTemperatura) || string.IsNullOrEmpty(renovacionAire) || string.IsNullOrEmpty(usuario))
            {
                throw new ArgumentException("Los parámetros no pueden ser nulos o vacíos.");
            }

            // Filtro para actualizar o insertar los datos climáticos óptimos
            var filter = new BsonDocument();
            var update = Builders<BsonDocument>.Update
                .Set("optimoTemperatura", optimoTemperatura)
                .Set("renovacionAire", renovacionAire);

            var options = new UpdateOptions { IsUpsert = true };// Opciones para la operación de actualización

            // Realiza la actualización en la colección de datos climáticos óptimos
            await _climaticaOptCollection.UpdateOneAsync(filter, update, options);

            // Crear y empezar el hilo para registrar cambios
            var registroCambioThread = new RegistroCambioThread(_registroCollection, _climaticaOptCollection, usuario, optimoTemperatura, renovacionAire);
            registroCambioThread.StartClimatica();
        }
    }
}
