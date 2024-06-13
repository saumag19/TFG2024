using TFGAndroid.Models;

namespace TFGAndroid.Pages;

public partial class MenuLateral : ContentView
{
    String pagina = null;// Variable para controlar la p�gina actual
    private Usuario _usuario;// Variable para almacenar el usuario

    // Constructor de la clase
    public MenuLateral()
	{
		InitializeComponent();// Inicializa los componentes visuales del men� lateral
    }

    // M�todo invocado al tocar en la secci�n clim�tica del men�
    private async void onTapClimatica(object sender, EventArgs e)
    {
        if (pagina != "climatica")
        {
            var faseClimatica = new FaseClimatica(_usuario);// Crea una instancia de la p�gina FaseClimatica
            await Navigation.PushAsync(faseClimatica);// Navega hacia la p�gina sin animaci�n
        }
    }

    // M�todo invocado al tocar en la secci�n hidr�ulica del men�
    private async void onTapHidraulica(object sender, EventArgs e)
    {
        if (pagina != "hidraulica")
        {
            var faseHidraulica = new FaseHidraulica(_usuario);// Crea una instancia de la p�gina FaseHidraulica
            await Navigation.PushAsync(faseHidraulica);// Navega hacia la p�gina
        }
    }

    // M�todo invocado al tocar en la secci�n laboratorio del men�
    private async void onTapLaboratorio(object sender, EventArgs e)
    {
        if (pagina != "laboratorio")
        {
            var faseLaboratorio = new FaseLaboratorio(_usuario);// Crea una instancia de la p�gina FaseLaboratorio
            await Navigation.PushAsync(faseLaboratorio);// Navega hacia la p�gina
        }

    }

    // M�todo invocado al tocar en la secci�n lum�nica del men�
    private async void onTapLuminica(object sender, EventArgs e)
    {
        if (pagina != "luminica")
        {
            var faseLuminica = new FaseLuminica(_usuario);// Crea una instancia de la p�gina FaseLuminica
            await Navigation.PushAsync(faseLuminica);// Navega hacia la p�gina
        }
    }

    // M�todo invocado al tocar en la secci�n de red del men�
    private async void onTapRed(object sender, EventArgs e)
    {
        if (pagina != "red")
        {
            var faseRed = new FaseRed(_usuario);// Crea una instancia de la p�gina FaseRed
            await Navigation.PushAsync(faseRed); // Navega hacia la p�gina
        }
    }

    // M�todo invocado al tocar en la secci�n de administrador del men�
    private async void onTapAdministrador(object sender, EventArgs e)
    {
        if (pagina != "administrador")
        {
            var administrador = new Administrador(_usuario);// Crea una instancia de la p�gina Administrador

            await Navigation.PushAsync(administrador);// Navega hacia la p�gina
        }

    }

    // M�todo para establecer el usuario en el men� lateral
    public void setUsuario(Usuario usuario)
    {
        _usuario = usuario;// Asigna el usuario recibido a la variable local
        // Si el usuario no es administrador, oculta el bot�n de administrador
        if (usuario.Type != "admin")
        {
            boton6.IsVisible = false;
        }
    }

    // M�todo para mostrar el men� lateral y establecer la p�gina actual
    public void Show(String page)
    {
        this.IsVisible = true; // Muestra el popup
        pagina = page;
    }

    // M�todo para ocultar el men� lateral y establecer la p�gina actual
    public void UnShow(String page)
    {
        this.IsVisible = false; // Muestra el popup
        pagina = page;
    }

    // M�todo invocado al hacer clic en el bot�n de cerrar sesi�n
    private void OnClickCerrarSesion(object sender, EventArgs e)
    {
        // Navega hacia la p�gina de inicio de sesi�n al cerrar sesi�n
        var faseClimatica = new NavigationPage(new InicioSesion());
        Application.Current.MainPage = faseClimatica;
    }
}