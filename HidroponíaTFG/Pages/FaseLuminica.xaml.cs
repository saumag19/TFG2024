using HidroponíaTFG.Database;
using HidroponíaTFG.Pages;
using HidroponíaTFG.Models;

namespace HidroponíaTFG.Pages;

public partial class FaseLuminica : ContentPage
{
    private MonitorLuz _monitorLuz;
    private Usuario _usuario;

    public FaseLuminica(Usuario usuario)
    {
        _usuario = usuario;

        InitializeComponent();

        menulateral.setUsuario(_usuario);
        if (usuario.Type == "invit")
        {
            btn10.IsEnabled = false;
            entry1.IsEnabled = false;
            entry2.IsEnabled = false;
        }

        _monitorLuz = new MonitorLuz(this);
        _monitorLuz.StartMonitoring();

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

    private void MostrarPopup(object sender, EventArgs e)
    {
        popup.Show();
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

        await _monitorLuz.UpdateLuzOptStatus(nivel, potencia);
    }
}
