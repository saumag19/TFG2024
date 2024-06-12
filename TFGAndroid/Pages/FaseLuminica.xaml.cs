using TFGAndroid.Database;
using TFGAndroid.Models;

namespace TFGAndroid.Pages;

public partial class FaseLuminica : ContentPage
{
    private Usuario _usuario;
    private MonitorLuz _monitorLuz;
    public FaseLuminica(Usuario usuario)
	{
		InitializeComponent();
        _usuario = usuario;
        menulateral.setUsuario(_usuario);
        _monitorLuz = new MonitorLuz(this);
        _monitorLuz.StartMonitoring();

        if (usuario.Type == "invit")
        {
            btn10.IsEnabled = false;
            entry1.IsEnabled = false;
            entry2.IsEnabled = false;
        }

        btn10.Clicked += AplicarCambios;
    }

    private async void ToggleMenu(object sender, EventArgs e)
    {
        if (menulateral.IsVisible)
        {
            menulateral.UnShow("luminica");
        }
        else
        {
            menulateral.Show("luminica");
        }
    }

    private void OnSizeChanged(object sender, EventArgs e)
    {
        if (Width > Height)
        {
            // Landscape
            stackLayout.Rotation = 0;
            stackLayout2.Rotation = 0;
        }
        else
        {
            // Portrait
            stackLayout.Rotation = 90;
            stackLayout2.Rotation = 90;
        }
    }

    private void aparece(object sender, EventArgs e)
    {
        _monitorLuz.StartMonitoring();
    }

    private void desaparece(object sender, EventArgs e)
    {
        _monitorLuz.StopMonitoring();
    }

    private async void AplicarCambios(object sender, EventArgs e)
    {
        var nivel = entry1.Text;
        var potencia = entry2.Text;

        await _monitorLuz.UpdateLuzOptStatus(nivel, potencia,_usuario);
    }
}