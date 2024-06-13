using TFGAndroid.Database;
using MongoDB.Bson;

using System.Text.RegularExpressions;

namespace TFGAndroid.Pages;

public partial class Registro : ContentPage
{
    private BBDD _bbdd;// Instancia de la base de datos
    ContentPage inicio;// P�gina de inicio de sesi�n

    // Constructor de la clase
    public Registro()
	{
		InitializeComponent();// Inicializa los componentes visuales de la p�gina
        _bbdd = new BBDD();// Inicializa la instancia de la base de datos
        inicio = new InicioSesion();// Crea una instancia de la p�gina de inicio de sesi�n
    }

    // M�todo invocado al hacer clic en el bot�n de iniciar sesi�n
    private async void OnButtonIniciarClicked(object sender, EventArgs e)
    {
        var inicio = new InicioSesion();// Crea una instancia de la p�gina de inicio de sesi�n
        await Navigation.PushAsync(inicio);// Navega hacia la p�gina de inicio de sesi�n

    }

    // M�todo invocado al hacer clic en el bot�n de registrarse
    private void OnButtonRegistrarseClicked(object sender, EventArgs e)
    {
        // Obtener los datos del formulario
        string nombre = EntryNombre.Text;
        string contrase�a = EntryContrase�a.Text;
        string email = EntryEmail.Text;
        string tipoUsuario = "invit";

        // Verificar que la contrase�a y la contrase�a repetida sean la misma
        if (contrase�a != EntryContrase�a2.Text)
        {
            // Mostrar un mensaje de error si alg�n dato est� vac�o
            DisplayAlert("Error", "Las contrase�as no coinciden.", "Aceptar");
            return;
        }
        else
        {
            // Verificar que el email tenga una estructura v�lida
            if (!EsEmailValido(email))
            {
                DisplayAlert("Error", "El correo electr�nico no es v�lido.", "Aceptar");
                return;
            }
            // Verificar que ning�n dato est� vac�o
            if (string.IsNullOrEmpty(nombre) || string.IsNullOrEmpty(contrase�a) || string.IsNullOrEmpty(email))
            {
                // Mostrar un mensaje de error si alg�n dato est� vac�o
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
                        { "pass", contrase�a },
                        { "type", tipoUsuario }
                    };
                    try
                    {
                        // Insertar el nuevo usuario en la base de datos
                        _bbdd.InsertarUsuario(nuevoUsuario);

                        // Mostrar un mensaje de �xito
                        DisplayAlert("�xito", "Usuario insertado correctamente.", "Aceptar");

                        OnButtonIniciarClicked(sender, e);// Llama al m�todo de inicio de sesi�n despu�s de registrar correctamente
                    }
                    catch (Exception ex)
                    {
                        // Mostrar un mensaje de error si ocurre alg�n problema durante la inserci�n
                        DisplayAlert("Error", $"Ocurri� un error al insertar el usuario: {ex.Message}", "Aceptar");
                    }
                }
            }
        }
    }

    // M�todo para validar si un email tiene una estructura v�lida
    private bool EsEmailValido(string email)
    {
        // Expresi�n regular para validar el email
        string patronEmail = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
        return Regex.IsMatch(email, patronEmail);// Retorna true si el email cumple con el patr�n
    }
}