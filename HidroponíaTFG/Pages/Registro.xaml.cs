using HidroponíaTFG.Database;
using MongoDB.Bson;
using System.Text.RegularExpressions;

namespace HidroponíaTFG.Pages;

public partial class Registro : ContentPage
{
    private BBDD _bbdd;
    ContentPage inicio;
    public Registro()
	{
		InitializeComponent();
        _bbdd = new BBDD();
        inicio = new InicioSesion();
    }

    private async void OnButtonIniciarClicked(object sender, EventArgs e)
    {
        var inicio = new InicioSesion();
        await Navigation.PushAsync(inicio);

    }

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
                }else {
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

                        OnButtonIniciarClicked(sender, e);
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
    private bool EsEmailValido(string email)
    {
        // Expresión regular para validar el email
        string patronEmail = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
        return Regex.IsMatch(email, patronEmail);
    }
}