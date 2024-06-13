using TFGAndroid.Database;
using TFGAndroid.Models;

namespace TFGAndroid.Pages;

public partial class FaseClimatica : ContentPage
{
    private Usuario _usuario; // Declaraci�n del objeto Usuario
    private MonitorClimatica _monitorClimatica; // Declaraci�n del objeto MonitorClimatica

    // Constructor de la clase FaseClimatica que recibe un objeto Usuario como par�metro
    public FaseClimatica(Usuario usuario)
	{
		InitializeComponent();// Inicializa los componentes de la interfaz

        _usuario = usuario;// Asigna el usuario pasado como par�metro a la variable privada _usuario
        menulateral.setUsuario(_usuario);// Configura el men� lateral con el usuario actual

        // Si el tipo de usuario es invitado, deshabilita ciertos elementos de la interfaz
        if (usuario.Type == "invit")
        {
            btn1.IsEnabled = false;
            entry1.IsEnabled = false;
            entry2.IsEnabled = false;
        }

        // Inicializa el objeto MonitorClimatica y comienza a monitorear
        _monitorClimatica = new MonitorClimatica(this);
        _monitorClimatica.StartMonitoring();
    }

    // M�todo para alternar la visibilidad del men� lateral
    private async void ToggleMenu(object sender, EventArgs e)
    {
        if (menulateral.IsVisible)// Si el men� lateral es visible
        {
            menulateral.UnShow("climatica");// Oculta el men� lateral en la secci�n "climatica"
        }
        else // Si el men� lateral no es visible
        {
            menulateral.Show("climatica");// Muestra el men� lateral en la secci�n "climatica"
        }
    }

    // M�todo que se llama cuando la p�gina aparece, comenzando el monitoreo
    private void aparece(object sender, EventArgs e)
    {
        _monitorClimatica.StartMonitoring();
    }

    // M�todo que se llama cuando la p�gina desaparece, deteniendo el monitoreo
    private void desaparece(object sender, EventArgs e)
    {
        _monitorClimatica.StopMonitoring();
    }

    // M�todo que se llama cuando se hace clic en el bot�n btn1
    private async void btn1_Clicked(object sender, EventArgs e)
    {
        var optimoTemperatura = entry1.Text;
        var renovacionAire = entry2.Text;

        await _monitorClimatica.SaveClimaticaData(optimoTemperatura, renovacionAire, _usuario.Nombre);
    }
}