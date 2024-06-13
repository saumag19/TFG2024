using Microcharts;
using SkiaSharp;
using HidroponíaTFG.Database;
using HidroponíaTFG.Models;

namespace HidroponíaTFG.Pages
{
    public partial class FaseHidraulica : ContentPage
    {
        private Usuario _usuario;// Variable privada para almacenar información del usuario
        private MonitorHidraulica _monitorHidraulica;// Instancia del monitor hidráulico para el seguimiento
        ChartEntry[] entries2 = new[]// Arreglo de entradas para los gráficos de ejemplo
        {
            new ChartEntry(456)
            {
                Label = "Dia 2",
                ValueLabel = "648",
                Color = SKColor.Parse("#7cffcb")
            },
            new ChartEntry(345)
            {
                Label = "Dia 3",
                ValueLabel = "428",
                Color = SKColor.Parse("#7cffcb")
            },
            new ChartEntry(234)
            {
                Label = "Dia 4",
                ValueLabel = "cola",
                Color = SKColor.Parse("#7cffcb")
            },
            new ChartEntry(456)
            {
                Label = "Dia 5",
                ValueLabel = "214",
                Color = SKColor.Parse("#7cffcb")
            },
            new ChartEntry(123)
            {
                Label = "Dia 5",
                ValueLabel = "214",
                Color = SKColor.Parse("#7cffcb")
            }
        };

        // Constructor que recibe un usuario al inicializar la página
        public FaseHidraulica(Usuario usuario)
        {
            _usuario = usuario;// Asigna el usuario recibido
            InitializeComponent();// Inicializa los componentes visuales de la página
            menulateral.setUsuario(_usuario);// Configura el usuario en el menú lateral

            // Si el tipo de usuario es invitado, deshabilita ciertos botones en la interfaz
            if (usuario.Type == "invit")
            {
                btn1.IsEnabled = false;
                btn2.IsEnabled = false;
                btn3.IsEnabled = false;
                btn4.IsEnabled = false;
                btn5.IsEnabled = false;
            }

            // Inicializa el monitor hidráulico asociado a esta página y configura los gráficos iniciales
            _monitorHidraulica = new MonitorHidraulica(this, chartView1, chartView2, chartView3,chartView5,chartView6);
            _monitorHidraulica.StartMonitoring();// Comienza el monitoreo hidráulico al iniciar la página

            // Asigna los datos iniciales a los gráficos de la página
            chartView1.Chart = new BarChart
            {
                Entries = entries2
            };
            chartView2.Chart = new LineChart
            {
                Entries = entries2
            };
            chartView3.Chart = new LineChart
            {
                Entries = entries2
            };
            chartView5.Chart = new LineChart
            {
                Entries = entries2
            };
            chartView6.Chart = new LineChart
            {
                Entries = entries2
            };
        }

        // Método para manejar el evento de mostrar/ocultar el menú lateral
        private async void ToggleMenu(object sender, EventArgs e)
        {
            if (menulateral.IsVisible)
            {
                menulateral.UnShow("hidraulica");// Oculta el menú lateral específico para esta sección
            }
            else
            {
                menulateral.Show("hidraulica");// Muestra el menú lateral específico para esta sección
            }
        }

        // Método para manejar el evento de aparecer nuevamente en la página
        private void aparece(object sender, EventArgs e)
        {
            _monitorHidraulica.StartMonitoring();  // Reinicia el monitoreo hidráulico al aparecer la página
        }

        // Método para manejar el evento de desaparecer de la página
        private void desaparece(object sender, EventArgs e)
        {
            _monitorHidraulica.StopMonitoring();  // Detiene el monitoreo hidráulico al desaparecer la página
        }

        // Método para aplicar cambios en el nivel del depósito según lo ingresado por el usuario
        private async void AplicarCambiosNivelDeposito(object sender, EventArgs e)
        {
            var newNivelDeposito = ((Entry)((Button)sender).Parent.FindByName("entryNivelDeposito")).Text;
            await _monitorHidraulica.UpdateHidraulicaOpt("nivel_deposito", newNivelDeposito, _usuario.Nombre);
        }

        // Método para aplicar cambios en el nivel de nitrógeno según lo ingresado por el usuario
        private async void AplicarCambiosNitrogeno(object sender, EventArgs e)
        {
            var newNitrogeno = ((Entry)((Button)sender).Parent.FindByName("entryNitrogeno")).Text;
            await _monitorHidraulica.UpdateHidraulicaOpt("nitrogeno", newNitrogeno, _usuario.Nombre);
        }

        // Método para aplicar cambios en el nivel de potasio según lo ingresado por el usuario
        private async void AplicarCambiosPotasio(object sender, EventArgs e)
        {
            var newPotasio = ((Entry)((Button)sender).Parent.FindByName("entryPotasio")).Text;
            await _monitorHidraulica.UpdateHidraulicaOpt("potasio", newPotasio, _usuario.Nombre);
        }

        // Método para aplicar cambios en el nivel de fósforo según lo ingresado por el usuario
        private async void AplicarCambiosFosforo(object sender, EventArgs e)
        {
            var newFosforo = ((Entry)((Button)sender).Parent.FindByName("entryFosforo")).Text;
            await _monitorHidraulica.UpdateHidraulicaOpt("fosforo", newFosforo, _usuario.Nombre);
        }

        // Método para aplicar cambios en el nivel de oxígeno según lo ingresado por el usuario
        private async void AplicarCambiosOxigeno(object sender, EventArgs e)
        {
            var newOxigeno = ((Entry)((Button)sender).Parent.FindByName("entryOxigeno")).Text;
            await _monitorHidraulica.UpdateHidraulicaOpt("oxigeno", newOxigeno, _usuario.Nombre);
        }

        // Metodo para recargar los datos visibles
        private async void recargar(object sender, EventArgs e)
        {
            aparece(sender, e);
        }
    }
}
