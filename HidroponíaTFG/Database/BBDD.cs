using MongoDB.Bson;
using MongoDB.Driver;
using HidroponíaTFG.Models;
using BCrypt.Net;

namespace HidroponíaTFG.Database
{
    internal class BBDD
    {
        private IMongoDatabase _database;// Instancia de la base de datos MongoDB
        private IMongoCollection<BsonDocument> _usuariosCollection;// Colección de documentos de usuarios
        private IMongoCollection<BsonDocument> _registroCollection;// Colección de documentos de registros

        public BBDD()
        {
            // Inicializa el cliente MongoDB con la cadena de conexión y obtiene las colecciones
            var client = new MongoClient("mongodb://root:root@ac-pn6khua-shard-00-00.vprqszh.mongodb.net:27017,ac-pn6khua-shard-00-01.vprqszh.mongodb.net:27017,ac-pn6khua-shard-00-02.vprqszh.mongodb.net:27017/?ssl=true&replicaSet=atlas-a8s0cb-shard-0&authSource=admin&retryWrites=true&w=majority&appName=ProyectoTFG");
            _database = client.GetDatabase("ProyectoTFG");
            _usuariosCollection = _database.GetCollection<BsonDocument>("Usuarios");
            _registroCollection = _database.GetCollection<BsonDocument>("Registro");
        }

        public List<BsonDocument> ObtenerTodosUsuarios()
        {
            // Devuelve todos los documentos de la colección de usuarios
            return _usuariosCollection.Find(new BsonDocument()).ToList();
        }

        public List<string> ObtenerTodosRegistros()
        {
            // Devuelve todos los documentos de la colección de registros como cadenas de texto
            var registros = _registroCollection.Find(new BsonDocument()).ToList();
            var registrosString = new List<string>();
            foreach (var registro in registros)
            {
                registrosString.Add(registro.ToString()); // Convertir cada BsonDocument a string y agregar a la lista
            }
            return registrosString;
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
                // Cifra la contraseña del usuario 
                var contraseña = usuario.GetValue("pass").AsString;
                var contraseñaCifrada = BCrypt.Net.BCrypt.HashPassword(contraseña);
                usuario.Set("pass", contraseñaCifrada);
                // Si no existe un usuario con el mismo nombre, insertarlo en la colección
                _usuariosCollection.InsertOne(usuario);
            }
        }

        public void ActualizarUsuario(string nombreOriginal, BsonDocument usuario)
        {
            // Verifica si un usuario con el nuevo nombre ya existe, excluyendo el usuario actual
            var newNombre = usuario.GetValue("name").ToString();
            var originalNombre = nombreOriginal;

            var filtroNuevoNombre = Builders<BsonDocument>.Filter.Eq("name", newNombre);
            var filtroExcluirUsuarioActual = Builders<BsonDocument>.Filter.Ne("name", originalNombre);
            var filtroCombinado = filtroNuevoNombre & filtroExcluirUsuarioActual;
            var usuarioExistenteConNuevoNombre = _usuariosCollection.Find(filtroCombinado).FirstOrDefault();

            if (usuarioExistenteConNuevoNombre != null)
            {
                // Si el nuevo nombre ya existe, lanzar una excepción
                throw new InvalidOperationException("Ya existe un usuario con el mismo nombre.");
            }
            // Actualiza el usuario utilizando el nombre original como filtro
            var filtroOriginalNombre = Builders<BsonDocument>.Filter.Eq("name", originalNombre);
            _usuariosCollection.ReplaceOne(filtroOriginalNombre, usuario);
        }

        public Usuario ObtenerUsuarioPorNombre(string nombre)
        {
            // Obtiene un usuario por su nombre
            var filtro = Builders<BsonDocument>.Filter.Eq("name", nombre);
            var usuarioBson = _usuariosCollection.Find(filtro).FirstOrDefault();

            if (usuarioBson == null)
            {
                return null;
            }

            return BsonToUsuario(usuarioBson);// Convierte el BsonDocument a un objeto Usuario
        }

        public void EliminarUsuario(string nombre)
        {
            // Elimina un usuario por su nombre
            var filter = Builders<BsonDocument>.Filter.Eq("name", nombre);
            _usuariosCollection.DeleteOne(filter);
        }

        public List<Usuario> GetUsuarios()
        {
            // Devuelve una lista de objetos Usuario a partir de la colección de usuarios
            var collection = _database.GetCollection<BsonDocument>("Usuarios");
            var bsonUsuarios = collection.Find(new BsonDocument()).ToList();
            var usuarios = bsonUsuarios.Select(bson => BsonToUsuario(bson)).ToList();
            return usuarios;
        }

        private Usuario BsonToUsuario(BsonDocument bson)
        {
            // Convierte un BsonDocument a un objeto Usuario
            return new Usuario(
                bson["name"].AsString,
                bson["mail"].AsString,
                bson["pass"].AsString,
                bson["type"].AsString
            );
        }
        public bool VerificarUsuario(string nombre, string contraseña)
        {
            // Verifica las credenciales de un usuario
            var filtro = Builders<BsonDocument>.Filter.Eq("name", nombre);
            var usuario = _usuariosCollection.Find(filtro).FirstOrDefault();

            if (usuario == null)
            {
                return false; // Usuario no encontrado
            }

            var contraseñaCifrada = usuario.GetValue("pass").AsString;
            return BCrypt.Net.BCrypt.Verify(contraseña, contraseñaCifrada);// Verifica la contraseña
        }

        // Devuelve registros que coincidan con el término de búsqueda en un campo específico
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




    }
}
