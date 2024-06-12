using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Threading;

namespace HidroponíaTFG.Database
{
    internal class RegistroCambioThreadLuz
    {
        private readonly IMongoCollection<BsonDocument> _registroCollection;
        private readonly IMongoCollection<BsonDocument> _luminicaOptCollection;
        private readonly string _usuario;
        private readonly string _nivel;
        private readonly string _potencia;

        public RegistroCambioThreadLuz(IMongoCollection<BsonDocument> registroCollection, IMongoCollection<BsonDocument> luminicaOptCollection, string nivel, string potencia, string usuario)
        {
            _registroCollection = registroCollection;
            _luminicaOptCollection = luminicaOptCollection;
            _usuario = usuario;
            _nivel = nivel;
            _potencia = potencia;
        }

        public void Start()
        {
            Thread registroThread = new Thread(RegistrarCambios);
            registroThread.Start();
        }

        private void RegistrarCambios()
        {
            // Este método se ejecutará en un hilo separado
            var filter = new BsonDocument(); // Asume que solo hay un documento para actualizar
            var update = Builders<BsonDocument>.Update
                .Set("nivel", _nivel)
                .Set("potencia", _potencia);

            var options = new UpdateOptions { IsUpsert = true };

            // Obtener el documento antes de la actualización
            var oldDocument = _luminicaOptCollection.Find(filter).FirstOrDefault();

            // Realizar la actualización
            _luminicaOptCollection.UpdateOne(filter, update, options);

            // Obtener el documento después de la actualización
            var newDocument = _luminicaOptCollection.Find(filter).FirstOrDefault();

            // Crear el registro del cambio
            var registro = new BsonDocument
            {
                { "usuario", _usuario },
                    { "campo1", "nivel" },
                    { "valorAntiguo1", oldDocument?["nivel"] ?? BsonNull.Value },
                    { "valorNuevo1", newDocument["nivel"] },

                    { "campo2", "potencia" },
                    { "valorAntiguo2", oldDocument?["potencia"] ?? BsonNull.Value },
                    { "valorNuevo2", newDocument["potencia"] }



            };

            // Guardar el registro en la colección "Registro"
            _registroCollection.InsertOne(registro);
        }

    }
}
