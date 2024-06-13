using MongoDB.Bson;
using MongoDB.Driver;
using TFGAndroid.Models;
using BCrypt.Net;

namespace TFGAndroid.Database
{
    // Clase interna que gestiona la conexión y operaciones con la base de datos MongoDB
    internal class BBDD
    {
        private IMongoDatabase _database;// Instancia de la base de datos MongoDB
        private IMongoCollection<BsonDocument> _usuariosCollection;// Colección de usuarios en la base de datos
        private IMongoCollection<BsonDocument> _registroCollection;// Colección de registros en la base de datos

        // Constructor de la clase BBDD que inicializa la conexión con la base de datos
        public BBDD()
        {
            // Configuración de la conexión MongoDB Atlas
            var client = new MongoClient("mongodb://root:root@ac-pn6khua-shard-00-00.vprqszh.mongodb.net:27017,ac-pn6khua-shard-00-01.vprqszh.mongodb.net:27017,ac-pn6khua-shard-00-02.vprqszh.mongodb.net:27017/?ssl=true&replicaSet=atlas-a8s0cb-shard-0&authSource=admin&retryWrites=true&w=majority&appName=ProyectoTFG");
            _database = client.GetDatabase("ProyectoTFG");// Selecciona la base de datos "ProyectoTFG"
            _usuariosCollection = _database.GetCollection<BsonDocument>("Usuarios");// Obtiene la colección "Usuarios"
            _registroCollection = _database.GetCollection<BsonDocument>("Registro");// Obtiene la colección "Registro"
        }

        // Método para obtener todos los usuarios de la colección
        public List<BsonDocument> ObtenerTodosUsuarios()
        {
            return _usuariosCollection.Find(new BsonDocument()).ToList();// Consulta todos los documentos de la colección y los devuelve como lista de BsonDocument
        }

        // Método para insertar un nuevo usuario en la colección
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
                // Cifrar la contraseña antes de insertar el usuario
                var contraseña = usuario.GetValue("pass").AsString;
                var contraseñaCifrada = BCrypt.Net.BCrypt.HashPassword(contraseña);
                usuario.Set("pass", contraseñaCifrada);
                // Si no existe un usuario con el mismo nombre, insertarlo en la colección
                _usuariosCollection.InsertOne(usuario);
            }
        }

        // Método para obtener todos los registros de la colección "Registro"
        public List<string> ObtenerTodosRegistros()
        {
            var registros = _registroCollection.Find(new BsonDocument()).ToList();// Consulta todos los documentos de la colección
            var registrosString = new List<string>();

            // Convertir cada BsonDocument a string y agregarlo a la lista de registrosString
            foreach (var registro in registros)
            {
                registrosString.Add(registro.ToString());
            }
            return registrosString;// Devuelve la lista de registros como strings
        }

        // Método para actualizar un usuario en la colección "Usuarios"
        public void ActualizarUsuario(string nombreOriginal, BsonDocument usuario)
        {
            var newNombre = usuario.GetValue("name").ToString();// Obtener el nuevo nombre del usuario
            var originalNombre = nombreOriginal;// Obtener el nombre original del usuario

            // Verificar si ya existe un usuario con el nuevo nombre (excluyendo al usuario que se está modificando)
            var filtroNuevoNombre = Builders<BsonDocument>.Filter.Eq("name", newNombre);
            var filtroExcluirUsuarioActual = Builders<BsonDocument>.Filter.Ne("name", originalNombre);
            var filtroCombinado = filtroNuevoNombre & filtroExcluirUsuarioActual;
            var usuarioExistenteConNuevoNombre = _usuariosCollection.Find(filtroCombinado).FirstOrDefault();

            // Si ya existe un usuario con el nuevo nombre y no es el mismo usuario, lanzar una excepción o manejar el caso según sea necesario
            if (usuarioExistenteConNuevoNombre != null)
            {
                throw new InvalidOperationException("Ya existe un usuario con el mismo nombre.");
            }

            // Realizar la actualización utilizando el nombre original como filtro
            var filtroOriginalNombre = Builders<BsonDocument>.Filter.Eq("name", originalNombre);
            _usuariosCollection.ReplaceOne(filtroOriginalNombre, usuario);
        }

        // Método para obtener un usuario por su nombre de usuario
        public Usuario ObtenerUsuarioPorNombre(string nombre)
        {
            var filtro = Builders<BsonDocument>.Filter.Eq("name", nombre);// Filtro para buscar el usuario por nombre
            var usuarioBson = _usuariosCollection.Find(filtro).FirstOrDefault();// Consulta el usuario en la colección

            if (usuarioBson == null)
            {
                return null;// Si no se encuentra el usuario, devuelve null
            }

            return BsonToUsuario(usuarioBson);// Convierte el BsonDocument a objeto Usuario y lo devuelve
        }

        // Método para eliminar un usuario de la colección "Usuarios"
        public void EliminarUsuario(string nombre)
        {
            var filter = Builders<BsonDocument>.Filter.Eq("name", nombre); // Filtro para eliminar el usuario por nombre
            _usuariosCollection.DeleteOne(filter);// Elimina el usuario de la colección
        }

        // Método para obtener todos los usuarios como objetos Usuario en lugar de BsonDocument
        public List<Usuario> GetUsuarios()
        {
            var collection = _database.GetCollection<BsonDocument>("Usuarios"); 
            var bsonUsuarios = collection.Find(new BsonDocument()).ToList();
            var usuarios = bsonUsuarios.Select(bson => BsonToUsuario(bson)).ToList();
            return usuarios;// Devuelve la lista de usuarios
        }

        // Método privado para convertir un BsonDocument a objeto Usuario
        private Usuario BsonToUsuario(BsonDocument bson)
        {
            // Crea y devuelve un nuevo objeto Usuario con los campos del BsonDocument
            return new Usuario(
                bson["name"].AsString,
                bson["mail"].AsString,
                bson["pass"].AsString,
                bson["type"].AsString
            );
        }

        // Método para obtener registros filtrados por un término de búsqueda en la colección "Registro"
        public List<string> ObtenerRegistrosFiltrados(string searchTerm)
        {
            var filtro = Builders<BsonDocument>.Filter.Regex("nombreDeTuCampoEnElBSON", new BsonRegularExpression(searchTerm, "i")); // Filtro de expresión regular para búsqueda insensible a mayúsculas y minúsculas
            var registros = _registroCollection.Find(filtro).ToList(); // Consulta los documentos que coinciden con el filtro
            var registrosString = new List<string>();
            foreach (var registro in registros)
            {
                registrosString.Add(registro.ToString()); // Convierte cada BsonDocument a string y lo agrega a la lista
            }
            return registrosString;// Devuelve la lista de registros como strings
        }

        // Método para verificar las credenciales de un usuario
        public bool VerificarUsuario(string nombre, string contraseña)
        {
            var filtro = Builders<BsonDocument>.Filter.Eq("name", nombre);// Filtro para buscar el usuario por nombre
            var usuario = _usuariosCollection.Find(filtro).FirstOrDefault();// Consulta el usuario en la colección

            if (usuario == null)
            {
                return false; // Si no se encuentra el usuario, devuelve false
            }

            var contraseñaCifrada = usuario.GetValue("pass").AsString;// Obtiene la contraseña cifrada del usuario
            return BCrypt.Net.BCrypt.Verify(contraseña, contraseñaCifrada);// Verifica la contraseña con BCrypt y devuelve el resultado
        }

    }
}
