using TFGAndroid.Database;
using TFGAndroid.Models;

namespace TFGAndroid.Pages;

public partial class FaseRed : ContentPage
{
    private MonitorRed _monitorRed;
    private Usuario _usuario;
    public FaseRed(Usuario usuario)
	{
		InitializeComponent();
        _usuario = usuario;
        menulateral.setUsuario(_usuario);
        _monitorRed = new MonitorRed(this);
    }

    private async void ToggleMenu(object sender, EventArgs e)
    {
        if (menulateral.IsVisible)
        {
            menulateral.UnShow("red");
        }
        else
        {
            menulateral.Show("red");
        }
    }

    private void aparece(object sender, EventArgs e)
    {
        _monitorRed.StartMonitoring();
    }

    private void desaparece(object sender, EventArgs e)
    {
        _monitorRed.StopMonitoring();
    }
}