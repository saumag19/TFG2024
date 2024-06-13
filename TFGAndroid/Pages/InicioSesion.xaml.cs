using TFGAndroid.Database;
using TFGAndroid.Models;

namespace TFGAndroid.Pages;

public partial class InicioSesion : ContentPage
{
    private BBDD _bbdd;// Instancia de la base de datos

    public InicioSesion()
	{
		InitializeComponent();// Inicializa los componentes de la página
        _bbdd = new BBDD();// Inicializa la instancia de la base de datos
    }

    // Método invocado al hacer clic en el botón para iniciar sesión
    private async void OnButtonIniciarClicked(object sender, EventArgs e)
    {
        Usuario usuario;// Variable para almacenar el usuario
        string nombreUsuario = nombreEntry.Text;// Obtiene el nombre de usuario ingresado
        string contrasena = contrasenaEntry.Text;// Obtiene la contraseña ingresada

        // Verifica si el nombre de usuario o la contraseña están vacíos
        if (string.IsNullOrEmpty(nombreUsuario) || string.IsNullOrEmpty(contrasena))
        {
            await DisplayAlert("Error", "Por favor, ingrese un nombre de usuario y una contraseña.", "OK");
            return;
        }

        // Obtiene el usuario desde la base de datos según el nombre de usuario ingresado
        usuario = _bbdd.ObtenerUsuarioPorNombre(nombreUsuario);

        // Verifica si el usuario existe
        if (usuario == null)
        {
            await DisplayAlert("Error", "El usuario no existe.", "OK");
        }
        else if (!BCrypt.Net.BCrypt.Verify(contrasena, usuario.Pass))
        {
            // Verifica si la contraseña ingresada no coincide con la almacenada
            await DisplayAlert("Error", "Contraseña incorrecta.", "OK");
        }
        else
        {
            // Si el usuario y la contraseña son válidos, crea la página de FaseRed con el usuario y la establece como página principal
            var faseRed = new NavigationPage(new FaseRed(usuario));
            Application.Current.MainPage = faseRed;

        }
    }

    // Método invocado al hacer clic en el botón para registrarse
    private async void OnButtonRegistrarseClicked(object sender, EventArgs e)
    {
        var registro = new Registro();// Crea una instancia de la página de registro
        await Navigation.PushAsync(registro);// Navega hacia la página de registro

    }
}