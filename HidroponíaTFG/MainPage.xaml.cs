using Microcharts;
using SkiaSharp;

namespace HidroponíaTFG
{
    public partial class MainPage : ContentPage
    {
        ChartEntry[] entries = new[]
        {
            new ChartEntry(567)
            {
                Label = "Dia 1",
                ValueLabel = "112",
                Color = SKColor.Parse("#00ff00")
            },
            new ChartEntry(456)
            {
                Label = "Dia 2",
                ValueLabel = "648",
                Color = SKColor.Parse("#00ff00")
            },
            new ChartEntry(345)
            {
                Label = "Dia 3",
                ValueLabel = "428",
                Color = SKColor.Parse("#00ff00")
            },
            new ChartEntry(234)
            {
                Label = "Dia 4",
                ValueLabel = "cola",
                Color = SKColor.Parse("#00ff00")
            },
            new ChartEntry(456)
            {
                Label = "Dia 5",
                ValueLabel = "214",
                Color = SKColor.Parse("#00ff00")
            }
        };

        ChartEntry[] entries2 = new[]
        {
            new ChartEntry(456)
            {
                Label = "Dia 2",
                ValueLabel = "648",
                Color = SKColor.Parse("#00ff00")
            },
            new ChartEntry(345)
            {
                Label = "Dia 3",
                ValueLabel = "428",
                Color = SKColor.Parse("#00ff00")
            },
            new ChartEntry(234)
            {
                Label = "Dia 4",
                ValueLabel = "cola",
                Color = SKColor.Parse("#00ff00")
            },
            new ChartEntry(456)
            {
                Label = "Dia 5",
                ValueLabel = "214",
                Color = SKColor.Parse("#00ff00")
            },
            new ChartEntry(123)
            {
                Label = "Dia 5",
                ValueLabel = "214",
                Color = SKColor.Parse("#00ff00")
            }
        };
        bool toggle = true;


        public MainPage()
        {
            InitializeComponent();



            chartView.Chart = new LineChart
            {
                Entries = entries
            };
            chartView1.Chart = new LineChart
            {
                Entries = entries2
            };
            chartView2.Chart = new LineChart
            {
                Entries = entries
            };
            chartView3.Chart = new LineChart
            {
                Entries = entries2
            };


            // Crear un temporizador de sistema
            System.Timers.Timer timer = new System.Timers.Timer(3000); // 3000 milisegundos = 3 segundos


            // Manejar el evento del temporizador
            timer.Elapsed += (sender, e) =>
            {
                // Cambiar los datos de los gráficos
                if (toggle)
                {
                    chartView.Chart = new LineChart { Entries = entries2, AnimationProgress = 1 };
                }
                else
                {
                    chartView.Chart = new LineChart { Entries = entries, IsAnimated = false };
                }

                // Alternar el valor de la variable toggle
                toggle = !toggle;
            };

            // Iniciar el temporizador
            timer.Start();



        }
    }

}
