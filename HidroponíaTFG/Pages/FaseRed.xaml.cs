namespace HidroponíaTFG.Pages;
using HidroponíaTFG.Database;
using HidroponíaTFG.Models;
public partial class FaseRed : ContentPage
{
    private MonitorRed _monitorRed; // Instancia del monitor de red
    private Usuario _usuario; // Variable para almacenar información del usuario

    // Constructor que recibe un usuario al inicializar la página
    public FaseRed(Usuario usuario)
	{ 
        _usuario = usuario;// Asigna el usuario recibido

        InitializeComponent(); // Inicializa los componentes visuales de la página
        menulateral.setUsuario(_usuario);// Configura el usuario en el menú lateral
        _monitorRed = new MonitorRed(this);// Inicializa el monitor de red asociado a esta página
    }

    // Método para manejar el evento de aparecer nuevamente en la página
    private void aparece(object sender, EventArgs e)
    {
        _monitorRed.StartMonitoring();// Inicia el monitoreo de la red al aparecer la página
    }

    // Método para manejar el evento de desaparecer de la página
    private void desaparece(object sender, EventArgs e)
    {
        _monitorRed.StopMonitoring();// Detiene el monitoreo de la red al desaparecer la página
    }

    // Método para manejar el evento de mostrar/ocultar el menú lateral
    private async void ToggleMenu(object sender, EventArgs e)
    {
        if (menulateral.IsVisible)
        {
            menulateral.UnShow("red");// Oculta el menú lateral específico para esta sección
        }
        else
        {
            menulateral.Show("red");// Muestra el menú lateral específico para esta sección
        }

    }

    // Metodo para recargar los datos visibles
    private async void recargar(object sender, EventArgs e)
    {
        aparece(sender, e);
    }

}