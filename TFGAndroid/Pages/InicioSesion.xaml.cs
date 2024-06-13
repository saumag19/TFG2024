using TFGAndroid.Database;
using TFGAndroid.Models;

namespace TFGAndroid.Pages;

public partial class InicioSesion : ContentPage
{
    private BBDD _bbdd;// Instancia de la base de datos

    public InicioSesion()
	{
		InitializeComponent();// Inicializa los componentes de la p�gina
        _bbdd = new BBDD();// Inicializa la instancia de la base de datos
    }

    // M�todo invocado al hacer clic en el bot�n para iniciar sesi�n
    private async void OnButtonIniciarClicked(object sender, EventArgs e)
    {
        Usuario usuario;// Variable para almacenar el usuario
        string nombreUsuario = nombreEntry.Text;// Obtiene el nombre de usuario ingresado
        string contrasena = contrasenaEntry.Text;// Obtiene la contrase�a ingresada

        // Verifica si el nombre de usuario o la contrase�a est�n vac�os
        if (string.IsNullOrEmpty(nombreUsuario) || string.IsNullOrEmpty(contrasena))
        {
            await DisplayAlert("Error", "Por favor, ingrese un nombre de usuario y una contrase�a.", "OK");
            return;
        }

        // Obtiene el usuario desde la base de datos seg�n el nombre de usuario ingresado
        usuario = _bbdd.ObtenerUsuarioPorNombre(nombreUsuario);

        // Verifica si el usuario existe
        if (usuario == null)
        {
            await DisplayAlert("Error", "El usuario no existe.", "OK");
        }
        else if (!BCrypt.Net.BCrypt.Verify(contrasena, usuario.Pass))
        {
            // Verifica si la contrase�a ingresada no coincide con la almacenada
            await DisplayAlert("Error", "Contrase�a incorrecta.", "OK");
        }
        else
        {
            // Si el usuario y la contrase�a son v�lidos, crea la p�gina de FaseRed con el usuario y la establece como p�gina principal
            var faseRed = new NavigationPage(new FaseRed(usuario));
            Application.Current.MainPage = faseRed;

        }
    }

    // M�todo invocado al hacer clic en el bot�n para registrarse
    private async void OnButtonRegistrarseClicked(object sender, EventArgs e)
    {
        var registro = new Registro();// Crea una instancia de la p�gina de registro
        await Navigation.PushAsync(registro);// Navega hacia la p�gina de registro

    }
}