using TFGAndroid.Database;
using TFGAndroid.Models;

namespace TFGAndroid.Pages;

public partial class FaseRed : ContentPage
{
    // Variables privadas para el monitor de red y el usuario
    private MonitorRed _monitorRed;
    private Usuario _usuario;

    // Constructor que recibe un usuario como par�metro
    public FaseRed(Usuario usuario)
	{
		InitializeComponent();// Inicializa los componentes de la p�gina
        _usuario = usuario;// Asigna el usuario recibido a la variable local
        menulateral.setUsuario(_usuario);// Configura el usuario en el men� lateral
        _monitorRed = new MonitorRed(this);// Inicializa el monitor de red
    }

    // M�todo invocado al cambiar el estado del men� lateral
    private async void ToggleMenu(object sender, EventArgs e)
    {
        if (menulateral.IsVisible)
        {
            menulateral.UnShow("red");// Oculta el men� lateral
        }
        else
        {
            menulateral.Show("red");// Muestra el men� lateral
        }
    }

    // M�todo invocado al aparecer la p�gina
    private void aparece(object sender, EventArgs e)
    {
        _monitorRed.StartMonitoring();// Inicia el monitoreo de red cuando la p�gina vuelve a estar visible
    }

    // M�todo invocado al desaparecer la p�gina
    private void desaparece(object sender, EventArgs e)
    {
        _monitorRed.StopMonitoring();// Detiene el monitoreo de red cuando la p�gina deja de ser visible
    }
}