using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Threading;

namespace HidroponíaTFG.Database
{
    public class RegistroCambioThreadLaboratorio
    {
        private readonly IMongoCollection<BsonDocument> _registroCollection;
        private readonly string _usuario;
        private readonly string _field;
        private readonly string _oldValue;
        private readonly string _newValue;

        public RegistroCambioThreadLaboratorio(IMongoCollection<BsonDocument> registroCollection, string usuario, string field, string oldValue, string newValue)
        {
            _registroCollection = registroCollection;
            _usuario = usuario;
            _field = field;
            _oldValue = oldValue;
            _newValue = newValue;
        }

        public void Start()
        {
            Thread thread = new Thread(RegistrarCambio);
            thread.Start();
        }

        private async void RegistrarCambio()
        {
            var cambio = new BsonDocument
            {
                { "usuario", _usuario },
                { "field", _field },
                { "oldValue", _oldValue },
                { "newValue", _newValue }
            };

            await _registroCollection.InsertOneAsync(cambio);
        }
    }
}
