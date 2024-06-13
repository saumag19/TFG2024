using TFGAndroid.Database;
using TFGAndroid.Models;

namespace TFGAndroid.Pages;

public partial class FaseLaboratorio : ContentPage
{
    // Variables privadas para el monitor de laboratorio y el usuario
    private MonitorLaboratorio _monitorLaboratorio;
    private Usuario _usuario;

    // Constructor que recibe un usuario como parámetro
    public FaseLaboratorio(Usuario usuario)
	{
		InitializeComponent();// Inicializa los componentes de la página
        _usuario = usuario;// Asigna el usuario recibido a la variable local
        menulateral.setUsuario(_usuario);// Configura el usuario en el menú lateral

        // Si el usuario es de tipo invitado, deshabilita ciertos botones y entradas
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
        // Inicializa el monitor de laboratorio y comienza el monitoreo
        _monitorLaboratorio = new MonitorLaboratorio(this);
        _monitorLaboratorio.StartMonitoring();
    }

    // Método invocado al cambiar el estado del menú lateral
    private async void ToggleMenu(object sender, EventArgs e)
    {
        if (menulateral.IsVisible)
        {
            menulateral.UnShow("laboratorio"); // Oculta el menú lateral
        }
        else
        {
            menulateral.Show("laboratorio"); // Muestra el menú lateral
        }
    }

    // Método invocado al aparecer la página
    private void aparece(object sender, EventArgs e)
    {
        _monitorLaboratorio.StartMonitoring();// Reinicia el monitoreo del laboratorio cuando la página vuelve a estar visible
    }

    // Método invocado al desaparecer la página
    private void desaparece(object sender, EventArgs e)
    {
        _monitorLaboratorio.StopMonitoring();// Detiene el monitoreo del laboratorio cuando la página deja de ser visible
    }

    // Método invocado al aplicar cambios en la semilla
    private async void AplicarCambiosSemilla(object sender, EventArgs e)
    {
        await _monitorLaboratorio.UpdateLaboratorioOpt("humedad_semilla", entry1.Text, _usuario.Nombre);
        await _monitorLaboratorio.UpdateLaboratorioOpt("oxigeno_semilla", entry2.Text, _usuario.Nombre);
        await _monitorLaboratorio.UpdateLaboratorioOpt("luz_semilla", entry3.Text, _usuario.Nombre);
        await _monitorLaboratorio.UpdateLaboratorioOpt("nutrientes_semilla", entry4.Text, _usuario.Nombre);
    }

    // Método invocado al aplicar cambios en la plantula
    private async void AplicarCambiosPlantula(object sender, EventArgs e)
    {
        await _monitorLaboratorio.UpdateLaboratorioOpt("humedad_plantula", entry5.Text, _usuario.Nombre);
        await _monitorLaboratorio.UpdateLaboratorioOpt("oxigeno_plantula", entry6.Text, _usuario.Nombre);
        await _monitorLaboratorio.UpdateLaboratorioOpt("luz_plantula", entry7.Text, _usuario.Nombre);
        await _monitorLaboratorio.UpdateLaboratorioOpt("nutrientes_plantula", entry8.Text, _usuario.Nombre);
    }

    // Métodos invocados al hacer clic en botones para cambiar la colección
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