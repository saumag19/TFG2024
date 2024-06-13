using TFGAndroid.Models;

namespace TFGAndroid.Pages;

public partial class MenuLateral : ContentView
{
    String pagina = null;// Variable para controlar la página actual
    private Usuario _usuario;// Variable para almacenar el usuario

    // Constructor de la clase
    public MenuLateral()
	{
		InitializeComponent();// Inicializa los componentes visuales del menú lateral
    }

    // Método invocado al tocar en la sección climática del menú
    private async void onTapClimatica(object sender, EventArgs e)
    {
        if (pagina != "climatica")
        {
            var faseClimatica = new FaseClimatica(_usuario);// Crea una instancia de la página FaseClimatica
            await Navigation.PushAsync(faseClimatica);// Navega hacia la página sin animación
        }
    }

    // Método invocado al tocar en la sección hidráulica del menú
    private async void onTapHidraulica(object sender, EventArgs e)
    {
        if (pagina != "hidraulica")
        {
            var faseHidraulica = new FaseHidraulica(_usuario);// Crea una instancia de la página FaseHidraulica
            await Navigation.PushAsync(faseHidraulica);// Navega hacia la página
        }
    }

    // Método invocado al tocar en la sección laboratorio del menú
    private async void onTapLaboratorio(object sender, EventArgs e)
    {
        if (pagina != "laboratorio")
        {
            var faseLaboratorio = new FaseLaboratorio(_usuario);// Crea una instancia de la página FaseLaboratorio
            await Navigation.PushAsync(faseLaboratorio);// Navega hacia la página
        }

    }

    // Método invocado al tocar en la sección lumínica del menú
    private async void onTapLuminica(object sender, EventArgs e)
    {
        if (pagina != "luminica")
        {
            var faseLuminica = new FaseLuminica(_usuario);// Crea una instancia de la página FaseLuminica
            await Navigation.PushAsync(faseLuminica);// Navega hacia la página
        }
    }

    // Método invocado al tocar en la sección de red del menú
    private async void onTapRed(object sender, EventArgs e)
    {
        if (pagina != "red")
        {
            var faseRed = new FaseRed(_usuario);// Crea una instancia de la página FaseRed
            await Navigation.PushAsync(faseRed); // Navega hacia la página
        }
    }

    // Método invocado al tocar en la sección de administrador del menú
    private async void onTapAdministrador(object sender, EventArgs e)
    {
        if (pagina != "administrador")
        {
            var administrador = new Administrador(_usuario);// Crea una instancia de la página Administrador

            await Navigation.PushAsync(administrador);// Navega hacia la página
        }

    }

    // Método para establecer el usuario en el menú lateral
    public void setUsuario(Usuario usuario)
    {
        _usuario = usuario;// Asigna el usuario recibido a la variable local
        // Si el usuario no es administrador, oculta el botón de administrador
        if (usuario.Type != "admin")
        {
            boton6.IsVisible = false;
        }
    }

    // Método para mostrar el menú lateral y establecer la página actual
    public void Show(String page)
    {
        this.IsVisible = true; // Muestra el popup
        pagina = page;
    }

    // Método para ocultar el menú lateral y establecer la página actual
    public void UnShow(String page)
    {
        this.IsVisible = false; // Muestra el popup
        pagina = page;
    }

    // Método invocado al hacer clic en el botón de cerrar sesión
    private void OnClickCerrarSesion(object sender, EventArgs e)
    {
        // Navega hacia la página de inicio de sesión al cerrar sesión
        var faseClimatica = new NavigationPage(new InicioSesion());
        Application.Current.MainPage = faseClimatica;
    }
}