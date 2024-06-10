using TFGAndroid.Database;
using TFGAndroid.Models;

namespace TFGAndroid.Pages;

public partial class InicioSesion : ContentPage
{
    private BBDD _bbdd;
    public InicioSesion()
	{
		InitializeComponent();
        _bbdd = new BBDD();
    }

    private async void OnButtonIniciarClicked(object sender, EventArgs e)
    {
        Usuario usuario;
        string nombreUsuario = nombreEntry.Text;
        string contrasena = contrasenaEntry.Text;

        if (string.IsNullOrEmpty(nombreUsuario) || string.IsNullOrEmpty(contrasena))
        {
            await DisplayAlert("Error", "Por favor, ingrese un nombre de usuario y una contraseña.", "OK");
            return;
        }

        usuario = _bbdd.ObtenerUsuarioPorNombre(nombreUsuario);

        if (usuario == null)
        {
            await DisplayAlert("Error", "El usuario no existe.", "OK");
        }
        else if (!BCrypt.Net.BCrypt.Verify(contrasena, usuario.Pass))
        {
            await DisplayAlert("Error", "Contraseña incorrecta.", "OK");
        }
        else
        {

            var faseClimatica = new NavigationPage(new FaseRed(usuario));
            Application.Current.MainPage = faseClimatica;

        }
    }

    private async void OnButtonRegistrarseClicked(object sender, EventArgs e)
    {
        var registro = new Registro();
        await Navigation.PushAsync(registro);

    }
}