
using HidroponíaTFG.Database;
using HidroponíaTFG.Models;
using Microsoft.Win32;

namespace HidroponíaTFG.Pages;

public partial class InicioSesion : ContentPage
{
    private BBDD _bbdd;// Instancia de la clase BBDD para interactuar con la base de datos

    // Constructor de la página de inicio de sesión
    public InicioSesion()
	{
		InitializeComponent();// Inicializa los componentes visuales de la página
        _bbdd = new BBDD();// Inicializa la instancia de la base de datos
    }

    // Método para manejar el evento de clic en el botón de iniciar sesión
    private async void OnButtonIniciarClicked(object sender, EventArgs e)
    {
        Usuario usuario;  // Variable para almacenar el usuario obtenido de la base de datos
        string nombreUsuario = nombreEntry.Text; // Obtiene el nombre de usuario ingresado
        string contrasena = contrasenaEntry.Text; // Obtiene la contraseña ingresada

        // Verifica que el nombre de usuario y la contraseña no estén vacíos
        if (string.IsNullOrEmpty(nombreUsuario) || string.IsNullOrEmpty(contrasena))
        {
            await DisplayAlert("Error", "Por favor, ingrese un nombre de usuario y una contraseña.", "OK");
            return;
        }

        usuario = _bbdd.ObtenerUsuarioPorNombre(nombreUsuario); // Obtiene el usuario de la base de datos por su nombre

        // Si el usuario no existe en la base de datos, muestra un mensaje de error
        if (usuario == null)
        {
            await DisplayAlert("Error", "El usuario no existe.", "OK");
        }
        // Si la contraseña ingresada no coincide con la almacenada en la base de datos, muestra un mensaje de error
        else if (!BCrypt.Net.BCrypt.Verify(contrasena, usuario.Pass))
        {
            await DisplayAlert("Error", "Contraseña incorrecta.", "OK");
        }
        else
        {
            // Si el inicio de sesión es exitoso, navega a la página FaseLuminica pasando el usuario como parámetro
            var faseClimatica = new NavigationPage(new FaseLuminica(usuario));
            Application.Current.MainPage = faseClimatica;
            
        }
    }
    // Método para manejar el evento de clic en el botón de registrarse
    private async void OnButtonRegistrarseClicked(object sender, EventArgs e)
    {
        var registro = new Registro(); // Crea una instancia de la página de registro
        await Navigation.PushAsync(registro); // Navega hacia la página de registro

    }
    
}