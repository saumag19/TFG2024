using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Threading;

namespace TFGAndroid.Database
{
    // Clase para registrar cambios relacionados con la climatización en MongoDB
    public class RegistroCambioThread
    {
        private readonly IMongoCollection<BsonDocument> _registroCollection;// Colección para registros de cambios
        private readonly IMongoCollection<BsonDocument> _climaticaOptCollection;// Colección para opciones óptimas de climatización
        private readonly string _usuario;// Nombre de usuario asociado al cambio
        private readonly string _optimoTemperatura;// Valor óptimo de temperatura a actualizar
        private readonly string _renovacionAire;// Valor de renovación de aire a actualizar

        // Constructor de la clase
        public RegistroCambioThread(IMongoCollection<BsonDocument> registroCollection, IMongoCollection<BsonDocument> climaticaOptCollection, string usuario, string optimoTemperatura, string renovacionAire)
        {
            _registroCollection = registroCollection;
            _climaticaOptCollection = climaticaOptCollection;
            _usuario = usuario;
            _optimoTemperatura = optimoTemperatura;
            _renovacionAire = renovacionAire;
        }

        // Método para iniciar el registro de cambios relacionados con climatización
        public void StartClimatica()
        {
            Thread registroThread = new Thread(RegistrarCambios);
            registroThread.Start();
        }

        // Método privado para realizar el registro de cambios
        private void RegistrarCambios()
        {
            // Este método se ejecutará en un hilo separado
            var filter = new BsonDocument(); // Asume que solo hay un documento para actualizar
            var update = Builders<BsonDocument>.Update
                .Set("optimoTemperatura", _optimoTemperatura)
                .Set("renovacionAire", _renovacionAire);

            var options = new UpdateOptions { IsUpsert = true };

            // Obtener el documento antes de la actualización
            var oldDocument = _climaticaOptCollection.Find(filter).FirstOrDefault();

            // Realizar la actualización
            _climaticaOptCollection.UpdateOne(filter, update, options);

            // Obtener el documento después de la actualización
            var newDocument = _climaticaOptCollection.Find(filter).FirstOrDefault();

            // Crear el registro del cambio
            var registro = new BsonDocument
            {
                { "usuario", _usuario },
                    { "campo1", "optimoTemperatura" },
                    { "valorAntiguo1", oldDocument?["optimoTemperatura"] ?? BsonNull.Value },
                    { "valorNuevo1", newDocument["optimoTemperatura"] },

                    { "campo2", "renovacionAire" },
                    { "valorAntiguo2", oldDocument?["renovacionAire"] ?? BsonNull.Value },
                    { "valorNuevo2", newDocument["renovacionAire"] }
                        
                    
                
            };

            // Guardar el registro en la colección "Registro"
            _registroCollection.InsertOne(registro);
        }
    }
}
