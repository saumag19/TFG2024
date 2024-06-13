using TFGAndroid.Database;
using TFGAndroid.Models;

namespace TFGAndroid.Pages;

public partial class FaseLuminica : ContentPage
{
    // Variables privadas para el usuario y el monitor de luz
    private Usuario _usuario;
    private MonitorLuz _monitorLuz;

    // Constructor que recibe un usuario como parámetro
    public FaseLuminica(Usuario usuario)
	{
		InitializeComponent();// Inicializa los componentes de la página
        _usuario = usuario;// Asigna el usuario recibido a la variable local
        menulateral.setUsuario(_usuario);// Configura el usuario en el menú lateral

        // Inicializa el monitor de luz y comienza el monitoreo
        _monitorLuz = new MonitorLuz(this);
        _monitorLuz.StartMonitoring();

        // Si el usuario es de tipo invitado, deshabilita ciertos botones y entradas
        if (usuario.Type == "invit")
        {
            btn10.IsEnabled = false;
            entry1.IsEnabled = false;
            entry2.IsEnabled = false;
        }

        // Asocia el evento de clic del botón btn10 al método AplicarCambios
        btn10.Clicked += AplicarCambios;
    }

    // Método invocado al cambiar el estado del menú lateral
    private async void ToggleMenu(object sender, EventArgs e)
    {
        if (menulateral.IsVisible)
        {
            menulateral.UnShow("luminica");// Oculta el menú lateral
        }
        else
        {
            menulateral.Show("luminica");// Muestra el menú lateral
        }
    }

    // Método invocado cuando cambia el tamaño de la página, para saber cuando está rotando el móvil / tablet
    private void OnSizeChanged(object sender, EventArgs e)
    {
        if (Width > Height)
        {
            stackLayout.Rotation = 0;
            stackLayout2.Rotation = 0;
        }
        else
        {
            stackLayout.Rotation = 90;
            stackLayout2.Rotation = 90;
        }
    }

    // Método invocado al aparecer la página
    private void aparece(object sender, EventArgs e)
    {
        _monitorLuz.StartMonitoring();// Reinicia el monitoreo de luz cuando la página vuelve a estar visible
    }

    // Método invocado al desaparecer la página
    private void desaparece(object sender, EventArgs e)
    {
        _monitorLuz.StopMonitoring();// Detiene el monitoreo de luz cuando la página deja de ser visible
    }

    // Método invocado al aplicar cambios en los valores de nivel y potencia de luz
    private async void AplicarCambios(object sender, EventArgs e)
    {
        // Obtiene los valores ingresados para nivel y potencia de luz
        var nivel = entry1.Text;
        var potencia = entry2.Text;

        // Actualiza el estado de luz
        await _monitorLuz.UpdateLuzOptStatus(nivel, potencia,_usuario);
    }
}