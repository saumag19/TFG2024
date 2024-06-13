using Microcharts;
using SkiaSharp;
using HidroponíaTFG.Database;
using HidroponíaTFG.Models;
using System.Xml.Linq;
using Microcharts.Maui;
namespace HidroponíaTFG.Pages;

public partial class FaseLaboratorio : ContentPage
{
    private MonitorLaboratorio _monitorLaboratorio;// Instancia del monitor de laboratorio
    private Usuario _usuario;// Variable para almacenar información del usuario

    // Arreglo de entradas para los gráficos de ejemplo
    ChartEntry[] entries2 = new[]
        {
            new ChartEntry(456)
            {
                ValueLabel = "648",
                Color = SKColor.Parse("#7cffcb")
            },
            new ChartEntry(345)
            {
                ValueLabel = "428",
                Color = SKColor.Parse("#7cffcb")
            },
            new ChartEntry(234)
            {
                ValueLabel = "234",
                Color = SKColor.Parse("#7cffcb")
            },
            new ChartEntry(456)
            {
                ValueLabel = "214",
                Color = SKColor.Parse("#7cffcb")
            },
            new ChartEntry(123)
            {
                ValueLabel = "214",
                Color = SKColor.Parse("#7cffcb")
            },
            new ChartEntry(456)
            {
                ValueLabel = "214",
                Color = SKColor.Parse("#7cffcb")
            },
            new ChartEntry(123)
            {
                ValueLabel = "214",
                Color = SKColor.Parse("#7cffcb")
            },
            new ChartEntry(456)
            {
                ValueLabel = "648",
                Color = SKColor.Parse("#7cffcb")
            },
            new ChartEntry(345)
            {
                ValueLabel = "428",
                Color = SKColor.Parse("#7cffcb")
            },
            new ChartEntry(234)
            {
                ValueLabel = "234",
                Color = SKColor.Parse("#7cffcb")
            },
            new ChartEntry(456)
            {
                ValueLabel = "214",
                Color = SKColor.Parse("#7cffcb")
            },
            new ChartEntry(123)
            {
                ValueLabel = "214",
                Color = SKColor.Parse("#7cffcb")
            },
            new ChartEntry(456)
            {
                ValueLabel = "214",
                Color = SKColor.Parse("#7cffcb")
            },
            new ChartEntry(123)
            {
                ValueLabel = "214",
                Color = SKColor.Parse("#7cffcb")
            },
            new ChartEntry(123)
            {
                ValueLabel = "214",
                Color = SKColor.Parse("#7cffcb")
            }
        };
    bool toggle = true;

    // Constructor que recibe un usuario al inicializar la página
    public FaseLaboratorio(Usuario usuario)
	{
        _usuario = usuario;// Asigna el usuario recibido
        InitializeComponent();// Inicializa los componentes visuales de la página
        menulateral.setUsuario(_usuario);// Configura el usuario en el menú lateral

        // Si el tipo de usuario es invitado, deshabilita ciertos botones en la interfaz
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

        // Configura el gráfico de tipo HalfRadialGaugeChart con las entradas proporcionadas
        chartView1.Chart = new HalfRadialGaugeChart
        {
            Entries = entries2,
        };

        // Inicializa el monitor de laboratorio asociado a esta página
        _monitorLaboratorio = new MonitorLaboratorio(this, chartView1);
        _monitorLaboratorio.StartMonitoring();// Comienza el monitoreo del laboratorio
    }
    // Método para manejar el evento de mostrar/ocultar el menú lateral
    private async void ToggleMenu(object sender, EventArgs e)
    {
        if (menulateral.IsVisible)
        {
            menulateral.UnShow("laboratorio");// Oculta el menú lateral específico para esta sección
        }
        else
        {
            menulateral.Show("laboratorio");// Muestra el menú lateral específico para esta sección
        }

    }

    // Método para manejar el evento de aparecer nuevamente en la página
    private void aparece(object sender, EventArgs e)
    {
        _monitorLaboratorio.StartMonitoring();// Reinicia el monitoreo del laboratorio al aparecer la página
    }

    // Método para manejar el evento de desaparecer de la página
    private void desaparece(object sender, EventArgs e)
    {
        _monitorLaboratorio.StopMonitoring();// Detiene el monitoreo del laboratorio al desaparecer la página
    }

    // Método para aplicar cambios en las condiciones de la semilla según lo ingresado por el usuario
    private async void AplicarCambiosSemilla(object sender, EventArgs e)
    {
        await _monitorLaboratorio.UpdateLaboratorioOpt("humedad_semilla", entry1.Text, _usuario.Nombre);
        await _monitorLaboratorio.UpdateLaboratorioOpt("oxigeno_semilla", entry2.Text, _usuario.Nombre);
        await _monitorLaboratorio.UpdateLaboratorioOpt("luz_semilla", entry3.Text, _usuario.Nombre);
        await _monitorLaboratorio.UpdateLaboratorioOpt("nutrientes_semilla", entry4.Text, _usuario.Nombre);
    }

    // Método para aplicar cambios en las condiciones de la plantula según lo ingresado por el usuario
    private async void AplicarCambiosPlantula(object sender, EventArgs e)
    {
        await _monitorLaboratorio.UpdateLaboratorioOpt("humedad_plantula", entry5.Text, _usuario.Nombre);
        await _monitorLaboratorio.UpdateLaboratorioOpt("oxigeno_plantula", entry6.Text, _usuario.Nombre);
        await _monitorLaboratorio.UpdateLaboratorioOpt("luz_plantula", entry7.Text, _usuario.Nombre);
        await _monitorLaboratorio.UpdateLaboratorioOpt("nutrientes_plantula", entry8.Text, _usuario.Nombre);
    }


    // Métodos para manejar los eventos de click en los botones
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

    // Metodo para recargar los datos visibles
    private async void recargar(object sender, EventArgs e)
    {
        aparece(sender, e);
    }


}