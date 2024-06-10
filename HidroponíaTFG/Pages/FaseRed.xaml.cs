namespace HidroponíaTFG.Pages;
using HidroponíaTFG.Database;
using HidroponíaTFG.Models;
public partial class FaseRed : ContentPage
{
    private MonitorRed _monitorRed;
    private Usuario _usuario;
    public FaseRed(Usuario usuario)
	{ 
        _usuario = usuario;

        InitializeComponent();
        menulateral.setUsuario(_usuario);
        _monitorRed = new MonitorRed(this);
    }

    private void aparece(object sender, EventArgs e)
    {
        _monitorRed.StartMonitoring();
    }

    private void desaparece(object sender, EventArgs e)
    {
        _monitorRed.StopMonitoring();
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

}