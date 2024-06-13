using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Threading;

namespace TFGAndroid.Database
{
    public class RegistroCambioThreadLaboratorio
    {
        private readonly IMongoCollection<BsonDocument> _registroCollection;// Colección para registros de cambios
        private readonly string _usuario;// Nombre de usuario asociado al cambio
        private readonly string _field;// Campo modificado
        private readonly string _oldValue;// Valor anterior del campo
        private readonly string _newValue;// Nuevo valor del campo

        // Constructor de la clase
        public RegistroCambioThreadLaboratorio(IMongoCollection<BsonDocument> registroCollection, string usuario, string field, string oldValue, string newValue)
        {
            _registroCollection = registroCollection;
            _usuario = usuario;
            _field = field;
            _oldValue = oldValue;
            _newValue = newValue;
        }

        // Método para iniciar el hilo de registro de cambio
        public void Start()
        {
            Thread thread = new Thread(RegistrarCambio);
            thread.Start();
        }

        // Método privado asincrónico para registrar el cambio
        private async void RegistrarCambio()
        {
            var cambio = new BsonDocument
            {
                { "usuario", _usuario },
                { "field", _field },
                { "oldValue", _oldValue },
                { "newValue", _newValue }
            };

            // Guardar el registro en la colección "Registro"
            await _registroCollection.InsertOneAsync(cambio);
        }
    }
}
