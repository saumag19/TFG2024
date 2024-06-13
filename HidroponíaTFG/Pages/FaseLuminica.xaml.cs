using HidroponíaTFG.Database;
using HidroponíaTFG.Pages;
using HidroponíaTFG.Models;

namespace HidroponíaTFG.Pages;

public partial class FaseLuminica : ContentPage
{
    private MonitorLuz _monitorLuz;// Instancia del monitor de luz
    private Usuario _usuario;// Variable para almacenar información del usuario

    // Constructor que recibe un usuario al inicializar la página
    public FaseLuminica(Usuario usuario)
    {
        _usuario = usuario;// Asigna el usuario recibido

        InitializeComponent();// Inicializa los componentes visuales de la página

        menulateral.setUsuario(_usuario);// Configura el usuario en el menú lateral

        // Si el tipo de usuario es invitado, deshabilita ciertos botones en la interfaz
        if (usuario.Type == "invit")
        {
            btn10.IsEnabled = false;
            entry1.IsEnabled = false;
            entry2.IsEnabled = false;
        }

        _monitorLuz = new MonitorLuz(this);// Inicializa el monitor de luz asociado a esta página
        _monitorLuz.StartMonitoring();// Comienza el monitoreo de la luz

        btn10.Clicked += AplicarCambios;// Asocia el evento de clic al botón btn10 para aplicar cambios
    }

    // Método para manejar el evento de mostrar/ocultar el menú lateral
    private async void ToggleMenu(object sender, EventArgs e)
    {
        if (menulateral.IsVisible)
        {
            menulateral.UnShow("luminica");// Oculta el menú lateral específico para esta sección
        }
        else
        {
            menulateral.Show("luminica");// Muestra el menú lateral específico para esta sección
        }
    }

    // Método para mostrar el popup al hacer clic en algún elemento
    private void MostrarPopup(object sender, EventArgs e)
    {
        popup.Show();
    }

    // Método para manejar el evento de aparecer nuevamente en la página
    private void aparece(object sender, EventArgs e)
    {
        _monitorLuz.StartMonitoring();// Reinicia el monitoreo de la luz al aparecer la página
    }

    // Método para manejar el evento de desaparecer de la página
    private void desaparece(object sender, EventArgs e)
    {
        _monitorLuz.StopMonitoring();// Detiene el monitoreo de la luz al desaparecer la página
    }

    // Método para aplicar cambios en las opciones de luz según lo ingresado por el usuario
    private async void AplicarCambios(object sender, EventArgs e)
    {
        var nivel = entry1.Text;// Obtiene el nivel ingresado desde entry1
        var potencia = entry2.Text;// Obtiene la potencia ingresada desde entry2

        await _monitorLuz.UpdateLuzOptStatus(nivel, potencia, _usuario);// Llama al método para actualizar los ajustes de luz
    }

    // Metodo para recargar los datos visibles
    private async void recargar(object sender, EventArgs e)
    {
        aparece(sender, e);
    }

}
