using MongoDB.Bson;
using MongoDB.Driver;
using TFGAndroid.Models;
using BCrypt.Net;

namespace TFGAndroid.Database
{
    internal class BBDD
    {
        private IMongoDatabase _database;
        private IMongoCollection<BsonDocument> _usuariosCollection;

        private IMongoCollection<BsonDocument> _registroCollection;

        public BBDD()
        {
            var client = new MongoClient("mongodb://root:root@ac-pn6khua-shard-00-00.vprqszh.mongodb.net:27017,ac-pn6khua-shard-00-01.vprqszh.mongodb.net:27017,ac-pn6khua-shard-00-02.vprqszh.mongodb.net:27017/?ssl=true&replicaSet=atlas-a8s0cb-shard-0&authSource=admin&retryWrites=true&w=majority&appName=ProyectoTFG");
            _database = client.GetDatabase("ProyectoTFG");
            _usuariosCollection = _database.GetCollection<BsonDocument>("Usuarios");

            _registroCollection = _database.GetCollection<BsonDocument>("Registro");
        }

        public List<BsonDocument> ObtenerTodosUsuarios()
        {
            return _usuariosCollection.Find(new BsonDocument()).ToList();
        }

        public void InsertarUsuario(BsonDocument usuario)
        {
            // Verificar si ya existe un usuario con el mismo nombre
            var filtro = Builders<BsonDocument>.Filter.Eq("name", usuario.GetValue("name"));
            var usuarioExistente = _usuariosCollection.Find(filtro).FirstOrDefault();

            if (usuarioExistente != null)
            {
                // Si ya existe un usuario con el mismo nombre, lanzar una excepción o manejar el caso según sea necesario
                throw new InvalidOperationException("Ya existe un usuario con el mismo nombre.");
            }
            else
            {
                var contraseña = usuario.GetValue("pass").AsString;
                var contraseñaCifrada = BCrypt.Net.BCrypt.HashPassword(contraseña);
                usuario.Set("pass", contraseñaCifrada);
                // Si no existe un usuario con el mismo nombre, insertarlo en la colección
                _usuariosCollection.InsertOne(usuario);
            }
        }
        public List<string> ObtenerTodosRegistros()
        {
            var registros = _registroCollection.Find(new BsonDocument()).ToList();
            var registrosString = new List<string>();
            foreach (var registro in registros)
            {
                registrosString.Add(registro.ToString()); // Convertir cada BsonDocument a string y agregar a la lista
            }
            return registrosString;
        }

        public void ActualizarUsuario(string nombreOriginal, BsonDocument usuario)
        {
            // Extract new name and original name
            var newNombre = usuario.GetValue("name").ToString();
            var originalNombre = nombreOriginal;

            // Check if a user with the new name exists (excluding the user being modified)
            var filtroNuevoNombre = Builders<BsonDocument>.Filter.Eq("name", newNombre);
            var filtroExcluirUsuarioActual = Builders<BsonDocument>.Filter.Ne("name", originalNombre);
            var filtroCombinado = filtroNuevoNombre & filtroExcluirUsuarioActual;
            var usuarioExistenteConNuevoNombre = _usuariosCollection.Find(filtroCombinado).FirstOrDefault();

            // If the new name exists and is not the original name, prevent the update
            if (usuarioExistenteConNuevoNombre != null)
            {
                throw new InvalidOperationException("Ya existe un usuario con el mismo nombre.");
            }

            // Perform the update using the original name as the filter
            var filtroOriginalNombre = Builders<BsonDocument>.Filter.Eq("name", originalNombre);
            _usuariosCollection.ReplaceOne(filtroOriginalNombre, usuario);
        }




        public Usuario ObtenerUsuarioPorNombre(string nombre)
        {
            var filtro = Builders<BsonDocument>.Filter.Eq("name", nombre);
            var usuarioBson = _usuariosCollection.Find(filtro).FirstOrDefault();

            if (usuarioBson == null)
            {
                return null;
            }

            return BsonToUsuario(usuarioBson);
        }

        public void EliminarUsuario(string nombre)
        {
            var filter = Builders<BsonDocument>.Filter.Eq("name", nombre);
            _usuariosCollection.DeleteOne(filter);
        }

        public List<Usuario> GetUsuarios()
        {
            var collection = _database.GetCollection<BsonDocument>("Usuarios");
            var bsonUsuarios = collection.Find(new BsonDocument()).ToList();
            var usuarios = bsonUsuarios.Select(bson => BsonToUsuario(bson)).ToList();
            return usuarios;
        }

        private Usuario BsonToUsuario(BsonDocument bson)
        {
            return new Usuario(
                bson["name"].AsString,
                bson["mail"].AsString,
                bson["pass"].AsString,
                bson["type"].AsString
            );
        }

        public List<string> ObtenerRegistrosFiltrados(string searchTerm)
        {
            var filtro = Builders<BsonDocument>.Filter.Regex("nombreDeTuCampoEnElBSON", new BsonRegularExpression(searchTerm, "i")); // "i" indica insensibilidad a mayúsculas y minúsculas
            var registros = _registroCollection.Find(filtro).ToList();
            var registrosString = new List<string>();
            foreach (var registro in registros)
            {
                registrosString.Add(registro.ToString()); // Convertir cada BsonDocument a string y agregar a la lista
            }
            return registrosString;
        }
        public bool VerificarUsuario(string nombre, string contraseña)
        {
            var filtro = Builders<BsonDocument>.Filter.Eq("name", nombre);
            var usuario = _usuariosCollection.Find(filtro).FirstOrDefault();

            if (usuario == null)
            {
                return false; // Usuario no encontrado
            }

            var contraseñaCifrada = usuario.GetValue("pass").AsString;
            return BCrypt.Net.BCrypt.Verify(contraseña, contraseñaCifrada);
        }

    }
}
