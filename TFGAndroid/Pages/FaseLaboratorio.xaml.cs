using TFGAndroid.Database;
using TFGAndroid.Models;

namespace TFGAndroid.Pages;

public partial class FaseLaboratorio : ContentPage
{
    private MonitorLaboratorio _monitorLaboratorio;
    private Usuario _usuario;
    public FaseLaboratorio(Usuario usuario)
	{
		InitializeComponent();
        _usuario = usuario;
        menulateral.setUsuario(_usuario);

        if (usuario.Type == "invit")
        {
            btnAplicar1.IsEnabled = false;
            btnAplicar2.IsEnabled = false;
            entry1.IsEnabled = false;
            entry2.IsEnabled = false;
            entry3.IsEnabled = false;
            entry4.IsEnabled = false;
            entry5.IsEnabled = false;
            entry6.IsEnabled = false;
            entry7.IsEnabled = false;
            entry8.IsEnabled = false;
        }
        _monitorLaboratorio = new MonitorLaboratorio(this);
        _monitorLaboratorio.StartMonitoring();
    }

    private async void ToggleMenu(object sender, EventArgs e)
    {
        if (menulateral.IsVisible)
        {
            menulateral.UnShow("laboratorio");
        }
        else
        {
            menulateral.Show("laboratorio");
        }
    }
    private void aparece(object sender, EventArgs e)
    {
        _monitorLaboratorio.StartMonitoring();
    }

    private void desaparece(object sender, EventArgs e)
    {
        _monitorLaboratorio.StopMonitoring();
    }

    private async void AplicarCambiosSemilla(object sender, EventArgs e)
    {
        await _monitorLaboratorio.UpdateLaboratorioOpt("humedad_semilla", entry1.Text);
        await _monitorLaboratorio.UpdateLaboratorioOpt("oxigeno_semilla", entry2.Text);
        await _monitorLaboratorio.UpdateLaboratorioOpt("luz_semilla", entry3.Text);
        await _monitorLaboratorio.UpdateLaboratorioOpt("nutrientes_semilla", entry4.Text);
    }

    private async void AplicarCambiosPlantula(object sender, EventArgs e)
    {
        await _monitorLaboratorio.UpdateLaboratorioOpt("humedad_plantula", entry5.Text);
        await _monitorLaboratorio.UpdateLaboratorioOpt("oxigeno_plantula", entry6.Text);
        await _monitorLaboratorio.UpdateLaboratorioOpt("luz_plantula", entry7.Text);
        await _monitorLaboratorio.UpdateLaboratorioOpt("nutrientes_plantula", entry8.Text);
    }

    private async void ClickButon1s(object sender, EventArgs e)
    {
        await _monitorLaboratorio.ChangeCollection("btn1s");
    }

    private async void ClickButon2s(object sender, EventArgs e)
    {
        await _monitorLaboratorio.ChangeCollection("btn2s");
    }

    private async void ClickButon3s(object sender, EventArgs e)
    {
        await _monitorLaboratorio.ChangeCollection("btn3s");
    }
    private async void ClickButon4s(object sender, EventArgs e)
    {
        await _monitorLaboratorio.ChangeCollection("btn4s");
    }

    private async void ClickButon5s(object sender, EventArgs e)
    {
        await _monitorLaboratorio.ChangeCollection("btn5s");
    }

    private async void ClickButon6s(object sender, EventArgs e)
    {
        await _monitorLaboratorio.ChangeCollection("btn6s");
    }

    private async void ClickButon1p(object sender, EventArgs e)
    {
        await _monitorLaboratorio.ChangeCollection("btn1p");
    }

    private async void ClickButon2p(object sender, EventArgs e)
    {
        await _monitorLaboratorio.ChangeCollection("btn2p");
    }

    private async void ClickButon3p(object sender, EventArgs e)
    {
        await _monitorLaboratorio.ChangeCollection("btn3p");
    }
    private async void ClickButon4p(object sender, EventArgs e)
    {
        await _monitorLaboratorio.ChangeCollection("btn4p");
    }

    private async void ClickButon5p(object sender, EventArgs e)
    {
        await _monitorLaboratorio.ChangeCollection("btn5p");
    }

    private async void ClickButon6p(object sender, EventArgs e)
    {
        await _monitorLaboratorio.ChangeCollection("btn6p");
    }
}