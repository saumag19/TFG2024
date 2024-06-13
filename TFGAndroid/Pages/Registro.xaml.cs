using TFGAndroid.Database;
using MongoDB.Bson;

using System.Text.RegularExpressions;

namespace TFGAndroid.Pages;

public partial class Registro : ContentPage
{
    private BBDD _bbdd;// Instancia de la base de datos
    ContentPage inicio;// Página de inicio de sesión

    // Constructor de la clase
    public Registro()
	{
		InitializeComponent();// Inicializa los componentes visuales de la página
        _bbdd = new BBDD();// Inicializa la instancia de la base de datos
        inicio = new InicioSesion();// Crea una instancia de la página de inicio de sesión
    }

    // Método invocado al hacer clic en el botón de iniciar sesión
    private async void OnButtonIniciarClicked(object sender, EventArgs e)
    {
        var inicio = new InicioSesion();// Crea una instancia de la página de inicio de sesión
        await Navigation.PushAsync(inicio);// Navega hacia la página de inicio de sesión

    }

    // Método invocado al hacer clic en el botón de registrarse
    private void OnButtonRegistrarseClicked(object sender, EventArgs e)
    {
        // Obtener los datos del formulario
        string nombre = EntryNombre.Text;
        string contraseña = EntryContraseña.Text;
        string email = EntryEmail.Text;
        string tipoUsuario = "invit";

        // Verificar que la contraseña y la contraseña repetida sean la misma
        if (contraseña != EntryContraseña2.Text)
        {
            // Mostrar un mensaje de error si algún dato está vacío
            DisplayAlert("Error", "Las contraseñas no coinciden.", "Aceptar");
            return;
        }
        else
        {
            // Verificar que el email tenga una estructura válida
            if (!EsEmailValido(email))
            {
                DisplayAlert("Error", "El correo electrónico no es válido.", "Aceptar");
                return;
            }
            // Verificar que ningún dato esté vacío
            if (string.IsNullOrEmpty(nombre) || string.IsNullOrEmpty(contraseña) || string.IsNullOrEmpty(email))
            {
                // Mostrar un mensaje de error si algún dato está vacío
                DisplayAlert("Error", "Todos los campos son obligatorios.", "Aceptar");
                return;
            }
            else
            {
                // Verificar si ya existe un usuario con el mismo nombre
                if (_bbdd.GetUsuarios().Any(u => u.Nombre == nombre))
                {
                    DisplayAlert("Error", "Ya existe un usuario con el mismo nombre.", "Aceptar");
                    return;
                }
                else
                {
                    // Crear un documento BSON con los datos del nuevo usuario
                    var nuevoUsuario = new BsonDocument
                    {
                        { "name", nombre },
                        { "mail", email },
                        { "pass", contraseña },
                        { "type", tipoUsuario }
                    };
                    try
                    {
                        // Insertar el nuevo usuario en la base de datos
                        _bbdd.InsertarUsuario(nuevoUsuario);

                        // Mostrar un mensaje de éxito
                        DisplayAlert("Éxito", "Usuario insertado correctamente.", "Aceptar");

                        OnButtonIniciarClicked(sender, e);// Llama al método de inicio de sesión después de registrar correctamente
                    }
                    catch (Exception ex)
                    {
                        // Mostrar un mensaje de error si ocurre algún problema durante la inserción
                        DisplayAlert("Error", $"Ocurrió un error al insertar el usuario: {ex.Message}", "Aceptar");
                    }
                }
            }
        }
    }

    // Método para validar si un email tiene una estructura válida
    private bool EsEmailValido(string email)
    {
        // Expresión regular para validar el email
        string patronEmail = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
        return Regex.IsMatch(email, patronEmail);// Retorna true si el email cumple con el patrón
    }
}