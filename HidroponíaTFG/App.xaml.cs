using HidroponíaTFG.Pages;

namespace HidroponíaTFG
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
