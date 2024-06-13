using TFGAndroid.Models;
using TFGAndroid.Database;

namespace TFGAndroid.Pages;

public partial class FaseHidraulica : ContentPage
{
    // Variables privadas para el usuario y el monitor hidráulico
    private Usuario _usuario;
    private MonitorHidraulica _monitorHidraulica;

    // Constructor que recibe un usuario como parámetro
    public FaseHidraulica(Usuario usuario)
	{
		InitializeComponent();// Inicializa los componentes de la página
        _usuario = usuario;// Asigna el usuario recibido a la variable local

        menulateral.setUsuario(_usuario);// Configura el usuario en el menú lateral

        // Si el usuario es de tipo invitado, deshabilita ciertos botones
        if (usuario.Type == "invit")
        {
            btn1.IsEnabled = false;
            btn2.IsEnabled = false;
            btn3.IsEnabled = false;
            btn4.IsEnabled = false;
            btn5.IsEnabled = false;
        }

        // Inicializa el monitor hidráulico y comienza el monitoreo
        _monitorHidraulica = new MonitorHidraulica(this);
        _monitorHidraulica.StartMonitoring();
    }

    // Método invocado al cambiar el estado del menú lateral
    private async void ToggleMenu(object sender, EventArgs e)
    {
        if (menulateral.IsVisible)
        {
            menulateral.UnShow("hidraulica");// Oculta el menú lateral
        }
        else
        {
            menulateral.Show("hidraulica");// Muestra el menú lateral
        }
    }

    // Método invocado al aparecer la página
    private void aparece(object sender, EventArgs e)
    {
        // Reinicia el monitoreo hidráulico cuando la página vuelve a estar visible
        _monitorHidraulica.StartMonitoring();
    }

    // Método invocado al desaparecer la página
    private void desaparece(object sender, EventArgs e)
    {
        // Detiene el monitoreo hidráulico cuando la página deja de ser visible
        _monitorHidraulica.StopMonitoring();
    }

    // Método invocado al aplicar cambios en el nivel de depósito
    private async void AplicarCambiosNivelDeposito(object sender, EventArgs e)
    {
        var newNivelDeposito = ((Entry)((Button)sender).Parent.FindByName("entryNivelDeposito")).Text;
        await _monitorHidraulica.UpdateHidraulicaOpt("nivel_deposito", newNivelDeposito, _usuario.Nombre);
    }

    // Método invocado al aplicar cambios en el nivel de nitrógeno
    private async void AplicarCambiosNitrogeno(object sender, EventArgs e)
    {
        var newNitrogeno = ((Entry)((Button)sender).Parent.FindByName("entryNitrogeno")).Text;
        await _monitorHidraulica.UpdateHidraulicaOpt("nitrogeno", newNitrogeno, _usuario.Nombre);
    }

    // Método invocado al aplicar cambios en el nivel de potasio
    private async void AplicarCambiosPotasio(object sender, EventArgs e)
    {
        var newPotasio = ((Entry)((Button)sender).Parent.FindByName("entryPotasio")).Text;
        await _monitorHidraulica.UpdateHidraulicaOpt("potasio", newPotasio, _usuario.Nombre);
    }

    // Método invocado al aplicar cambios en el nivel de fósforo
    private async void AplicarCambiosFosforo(object sender, EventArgs e)
    {
        var newFosforo = ((Entry)((Button)sender).Parent.FindByName("entryFosforo")).Text;
        await _monitorHidraulica.UpdateHidraulicaOpt("fosforo", newFosforo, _usuario.Nombre);
    }

    // Método invocado al aplicar cambios en el nivel de oxígeno
    private async void AplicarCambiosOxigeno(object sender, EventArgs e)
    {
        var newOxigeno = ((Entry)((Button)sender).Parent.FindByName("entryOxigeno")).Text;
        await _monitorHidraulica.UpdateHidraulicaOpt("oxigeno", newOxigeno, _usuario.Nombre);
    }

}