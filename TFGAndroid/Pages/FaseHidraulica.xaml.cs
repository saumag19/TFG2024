using TFGAndroid.Models;
using TFGAndroid.Database;

namespace TFGAndroid.Pages;

public partial class FaseHidraulica : ContentPage
{
    private Usuario _usuario;
    private MonitorHidraulica _monitorHidraulica;
    public FaseHidraulica(Usuario usuario)
	{
		InitializeComponent();
        _usuario = usuario;
        menulateral.setUsuario(_usuario);

        if (usuario.Type == "invit")
        {
            btn1.IsEnabled = false;
            btn2.IsEnabled = false;
            btn3.IsEnabled = false;
            btn4.IsEnabled = false;
            btn5.IsEnabled = false;
        }

        _monitorHidraulica = new MonitorHidraulica(this);
        _monitorHidraulica.StartMonitoring();
    }

    private async void ToggleMenu(object sender, EventArgs e)
    {
        if (menulateral.IsVisible)
        {
            menulateral.UnShow("hidraulica");
        }
        else
        {
            menulateral.Show("hidraulica");
        }
    }

    private void aparece(object sender, EventArgs e)
    {
        _monitorHidraulica.StartMonitoring();
    }

    private void desaparece(object sender, EventArgs e)
    {
        _monitorHidraulica.StopMonitoring();
    }

    private async void AplicarCambiosNivelDeposito(object sender, EventArgs e)
    {
        var newNivelDeposito = ((Entry)((Button)sender).Parent.FindByName("entryNivelDeposito")).Text;
        await _monitorHidraulica.UpdateHidraulicaOpt("nivel_deposito", newNivelDeposito);
    }

    private async void AplicarCambiosNitrogeno(object sender, EventArgs e)
    {
        var newNitrogeno = ((Entry)((Button)sender).Parent.FindByName("entryNitrogeno")).Text;
        await _monitorHidraulica.UpdateHidraulicaOpt("nitrogeno", newNitrogeno);
    }

    private async void AplicarCambiosPotasio(object sender, EventArgs e)
    {
        var newPotasio = ((Entry)((Button)sender).Parent.FindByName("entryPotasio")).Text;
        await _monitorHidraulica.UpdateHidraulicaOpt("potasio", newPotasio);
    }

    private async void AplicarCambiosFosforo(object sender, EventArgs e)
    {
        var newFosforo = ((Entry)((Button)sender).Parent.FindByName("entryFosforo")).Text;
        await _monitorHidraulica.UpdateHidraulicaOpt("fosforo", newFosforo);
    }

    private async void AplicarCambiosOxigeno(object sender, EventArgs e)
    {
        var newOxigeno = ((Entry)((Button)sender).Parent.FindByName("entryOxigeno")).Text;
        await _monitorHidraulica.UpdateHidraulicaOpt("oxigeno", newOxigeno);
    }

}