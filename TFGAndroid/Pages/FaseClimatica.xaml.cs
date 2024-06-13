using TFGAndroid.Database;
using TFGAndroid.Models;

namespace TFGAndroid.Pages;

public partial class FaseClimatica : ContentPage
{
    private Usuario _usuario; // Declaración del objeto Usuario
    private MonitorClimatica _monitorClimatica; // Declaración del objeto MonitorClimatica

    // Constructor de la clase FaseClimatica que recibe un objeto Usuario como parámetro
    public FaseClimatica(Usuario usuario)
	{
		InitializeComponent();// Inicializa los componentes de la interfaz

        _usuario = usuario;// Asigna el usuario pasado como parámetro a la variable privada _usuario
        menulateral.setUsuario(_usuario);// Configura el menú lateral con el usuario actual

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

    // Método para alternar la visibilidad del menú lateral
    private async void ToggleMenu(object sender, EventArgs e)
    {
        if (menulateral.IsVisible)// Si el menú lateral es visible
        {
            menulateral.UnShow("climatica");// Oculta el menú lateral en la sección "climatica"
        }
        else // Si el menú lateral no es visible
        {
            menulateral.Show("climatica");// Muestra el menú lateral en la sección "climatica"
        }
    }

    // Método que se llama cuando la página aparece, comenzando el monitoreo
    private void aparece(object sender, EventArgs e)
    {
        _monitorClimatica.StartMonitoring();
    }

    // Método que se llama cuando la página desaparece, deteniendo el monitoreo
    private void desaparece(object sender, EventArgs e)
    {
        _monitorClimatica.StopMonitoring();
    }

    // Método que se llama cuando se hace clic en el botón btn1
    private async void btn1_Clicked(object sender, EventArgs e)
    {
        var optimoTemperatura = entry1.Text;
        var renovacionAire = entry2.Text;

        await _monitorClimatica.SaveClimaticaData(optimoTemperatura, renovacionAire, _usuario.Nombre);
    }
}