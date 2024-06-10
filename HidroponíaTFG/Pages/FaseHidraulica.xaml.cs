using Microcharts;
using SkiaSharp;
using HidroponíaTFG.Database;
using HidroponíaTFG.Models;

namespace HidroponíaTFG.Pages
{
    public partial class FaseHidraulica : ContentPage
    {
        private Usuario _usuario;
        private MonitorHidraulica _monitorHidraulica;
        ChartEntry[] entries2 = new[]
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

        public FaseHidraulica(Usuario usuario)
        {
            _usuario = usuario;
            InitializeComponent();
            menulateral.setUsuario(_usuario);

            if (usuario.Type == "invit")
            {
                btn1.IsEnabled = false;
                btn2.IsEnabled = false;
                btn3.IsEnabled = false;
                btn4.IsEnabled = false;
                btn5.IsEnabled = false;
            }

            _monitorHidraulica = new MonitorHidraulica(this, chartView1, chartView2, chartView3,chartView5,chartView6);
            _monitorHidraulica.StartMonitoring();

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
}
