using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Threading;

namespace HidroponíaTFG.Database
{
    public class RegistroCambioThreadHidraulica
    {
        // Colecciones de MongoDB
        private readonly IMongoCollection<BsonDocument> _registroCollection;
        private readonly IMongoCollection<BsonDocument> _hidraulicaOptCollection;
        // Información del usuario y valores a actualizar
        private readonly string _usuario;
        private readonly string _field;
        private readonly string _newValue;

        // Constructor de la clase RegistroCambioThreadHidraulica que inicializa las colecciones de MongoDB y otros campos necesarios
        public RegistroCambioThreadHidraulica(IMongoCollection<BsonDocument> registroCollection, IMongoCollection<BsonDocument> hidraulicaOptCollection, string usuario, string field, string newValue)
        {
            _registroCollection = registroCollection;
            _hidraulicaOptCollection = hidraulicaOptCollection;
            _usuario = usuario;
            _field = field;
            _newValue = newValue;
        }

        // Método para iniciar el hilo de registro de cambios en la hidráulica
        public void Start()
        {
            Thread registroThread = new Thread(RegistrarCambios);
            registroThread.Start();
        }

        // Método que registra los cambios en la colección de configuración hidráulica
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
