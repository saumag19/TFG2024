using TFGAndroid.Database;
using TFGAndroid.Models;

namespace TFGAndroid.Pages;

public partial class FaseClimatica : ContentPage
{
    private Usuario _usuario;
    private MonitorClimatica _monitorClimatica;
    public FaseClimatica(Usuario usuario)
	{
		InitializeComponent();

        _usuario = usuario;
        menulateral.setUsuario(_usuario);
        if (usuario.Type == "invit")
        {
            btn1.IsEnabled = false;
            entry1.IsEnabled = false;
            entry2.IsEnabled = false;
        }

        _monitorClimatica = new MonitorClimatica(this);
        _monitorClimatica.StartMonitoring();
    }

    private async void ToggleMenu(object sender, EventArgs e)
    {
        if (menulateral.IsVisible)
        {
            menulateral.UnShow("climatica");
        }
        else
        {
            menulateral.Show("climatica");
        }
    }

    private void aparece(object sender, EventArgs e)
    {
        _monitorClimatica.StartMonitoring();
    }

    private void desaparece(object sender, EventArgs e)
    {
        _monitorClimatica.StopMonitoring();
    }

    private async void btn1_Clicked(object sender, EventArgs e)
    {
        var optimoTemperatura = entry1.Text;
        var renovacionAire = entry2.Text;

        await _monitorClimatica.SaveClimaticaData(optimoTemperatura, renovacionAire, _usuario.Nombre);
    }
}