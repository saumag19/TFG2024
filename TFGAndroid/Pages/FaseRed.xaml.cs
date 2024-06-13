using TFGAndroid.Database;
using TFGAndroid.Models;

namespace TFGAndroid.Pages;

public partial class FaseRed : ContentPage
{
    // Variables privadas para el monitor de red y el usuario
    private MonitorRed _monitorRed;
    private Usuario _usuario;

    // Constructor que recibe un usuario como parámetro
    public FaseRed(Usuario usuario)
	{
		InitializeComponent();// Inicializa los componentes de la página
        _usuario = usuario;// Asigna el usuario recibido a la variable local
        menulateral.setUsuario(_usuario);// Configura el usuario en el menú lateral
        _monitorRed = new MonitorRed(this);// Inicializa el monitor de red
    }

    // Método invocado al cambiar el estado del menú lateral
    private async void ToggleMenu(object sender, EventArgs e)
    {
        if (menulateral.IsVisible)
        {
            menulateral.UnShow("red");// Oculta el menú lateral
        }
        else
        {
            menulateral.Show("red");// Muestra el menú lateral
        }
    }

    // Método invocado al aparecer la página
    private void aparece(object sender, EventArgs e)
    {
        _monitorRed.StartMonitoring();// Inicia el monitoreo de red cuando la página vuelve a estar visible
    }

    // Método invocado al desaparecer la página
    private void desaparece(object sender, EventArgs e)
    {
        _monitorRed.StopMonitoring();// Detiene el monitoreo de red cuando la página deja de ser visible
    }
}