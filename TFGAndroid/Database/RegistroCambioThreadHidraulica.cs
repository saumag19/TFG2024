using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Threading;

namespace TFGAndroid.Database
{
    public class RegistroCambioThreadHidraulica
    {
        private readonly IMongoCollection<BsonDocument> _registroCollection;// Colección para registros de cambios
        private readonly IMongoCollection<BsonDocument> _hidraulicaOptCollection;// Colección para opciones óptimas de hidráulica
        private readonly string _usuario;// Nombre de usuario asociado al cambio
        private readonly string _field;// Campo a actualizar
        private readonly string _newValue;// Nuevo valor del campo a actualizar

        // Constructor de la clase
        public RegistroCambioThreadHidraulica(IMongoCollection<BsonDocument> registroCollection, IMongoCollection<BsonDocument> hidraulicaOptCollection, string usuario, string field, string newValue)
        {
            _registroCollection = registroCollection;
            _hidraulicaOptCollection = hidraulicaOptCollection;
            _usuario = usuario;
            _field = field;
            _newValue = newValue;
        }

        // Método para iniciar el hilo de registro de cambios
        public void Start()
        {
            Thread registroThread = new Thread(RegistrarCambios);
            registroThread.Start();
        }

        // Método privado para realizar el registro de cambios
        private void RegistrarCambios()
        {
            // Este método se ejecutará en un hilo separado
            var filter = new BsonDocument(); // Asume que solo hay un documento para actualizar
            var update = Builders<BsonDocument>.Update.Set(_field, _newValue);

            var options = new UpdateOptions { IsUpsert = true };

            // Obtener el documento antes de la actualización
            var oldDocument = _hidraulicaOptCollection.Find(filter).FirstOrDefault();

            // Realizar la actualización
            _hidraulicaOptCollection.UpdateOne(filter, update, options);

            // Obtener el documento después de la actualización
            var newDocument = _hidraulicaOptCollection.Find(filter).FirstOrDefault();

            // Crear el registro del cambio
            var registro = new BsonDocument
            {
                { "usuario", _usuario },
                { "campo", _field },
                { "valorAntiguo", oldDocument?[_field] ?? BsonNull.Value },
                { "valorNuevo", newDocument[_field] }
            };

            // Guardar el registro en la colección "Registro"
            _registroCollection.InsertOne(registro);
        }
    }
}
