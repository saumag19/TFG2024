using HidroponíaTFG.Database;
using HidroponíaTFG.Models;
using MongoDB.Bson;
using MongoDB.Driver;

namespace HidroponíaTFG.Pages;

public partial class FaseClimatica : ContentPage
{
    private Usuario _usuario;// Variable privada para almacenar información del usuario
    private MonitorClimatica _monitorClimatica;// Instancia del monitor climático para el seguimiento

    // Constructor que recibe un usuario al inicializar la página
    public FaseClimatica(Usuario usuario)
	{
        _usuario = usuario;// Asigna el usuario recibido
        InitializeComponent();// Inicializa los componentes visuales de la página
        menulateral.setUsuario(_usuario);// Configura el usuario en el menú lateral

        // Si el tipo de usuario es "invit", deshabilita ciertos controles en la interfaz
        if (usuario.Type == "invit")
        {
            btn1.IsEnabled = false;
            entry1.IsEnabled = false;
            entry2.IsEnabled = false;
        }

        _monitorClimatica = new MonitorClimatica(this);// Inicializa el monitor climático asociado a esta página
        _monitorClimatica.StartMonitoring();// Comienza el monitoreo climático al iniciar la página
    }

    // Método para manejar el evento de mostrar/ocultar el menú lateral
    private async void ToggleMenu(object sender, EventArgs e)
    {
        if (menulateral.IsVisible)
        {
            menulateral.UnShow("climatica");// Oculta el menú lateral específico para esta sección
        }
        else
        {
            menulateral.Show("climatica");// Muestra el menú lateral específico para esta sección
        }

    }

    // Método para manejar el evento de aparecer nuevamente en la página
    private void aparece(object sender, EventArgs e)
    {
        _monitorClimatica.StartMonitoring();
    }

    // Método para manejar el evento de desaparecer de la página
    private void desaparece(object sender, EventArgs e)
    {
        _monitorClimatica.StopMonitoring();
    }

    // Método para manejar el evento de clic en el botón 1
    private async void btn1_Clicked(object sender, EventArgs e)
    {
        var optimoTemperatura = entry1.Text;// Obtiene el valor de la entrada de temperatura óptima
        var renovacionAire = entry2.Text;// Obtiene el valor de la entrada de renovación de aire

        // Llama al método para guardar los datos climáticos en la base de datos
        await _monitorClimatica.SaveClimaticaData(optimoTemperatura, renovacionAire, _usuario.Nombre);
    }

    // Metodo para recargar los datos visibles
    private async void recargar(object sender, EventArgs e)
    {
        aparece(sender,e);
    }

}