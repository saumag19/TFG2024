using TFGAndroid.Pages;

namespace TFGAndroid
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new NavigationPage(new InicioSesion());
        }
    }
}
