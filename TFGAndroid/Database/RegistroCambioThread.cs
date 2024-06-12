using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Threading;

namespace TFGAndroid.Database
{
    public class RegistroCambioThread
    {
        private readonly IMongoCollection<BsonDocument> _registroCollection;
        private readonly IMongoCollection<BsonDocument> _climaticaOptCollection;
        private readonly string _usuario;
        private readonly string _optimoTemperatura;
        private readonly string _renovacionAire;

        public RegistroCambioThread(IMongoCollection<BsonDocument> registroCollection, IMongoCollection<BsonDocument> climaticaOptCollection, string usuario, string optimoTemperatura, string renovacionAire)
        {
            _registroCollection = registroCollection;
            _climaticaOptCollection = climaticaOptCollection;
            _usuario = usuario;
            _optimoTemperatura = optimoTemperatura;
            _renovacionAire = renovacionAire;
        }

        public void StartClimatica()
        {
            Thread registroThread = new Thread(RegistrarCambios);
            registroThread.Start();
        }

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
