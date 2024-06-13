using HidroponíaTFG.Models;

namespace HidroponíaTFG.Pages;

public partial class MenuLateral : ContentView
{
    String pagina = null;// Variable para almacenar la página actualmente mostrada
    private Usuario _usuario;// Variable para almacenar el usuario actual

    // Constructor de la clase MenuLateral
    public MenuLateral( )
    {
        InitializeComponent();// Inicializa los componentes visuales del menú lateral

    }
    // Método para establecer el usuario actual en el menú lateral
    public void setUsuario(Usuario usuario)
    {
        _usuario = usuario;// Asigna el usuario recibido al campo privado _usuario
        btn_cerrar.Text = "Cerrar Sesión";// Configura el texto del botón para cerrar sesión
        // Si el usuario no es administrador, oculta el botón correspondiente
        if ( usuario.Type != "admin")
        {
            boton6.IsVisible = false;
        }
    }

    // Método para manejar el evento de toque en la sección climática del menú
    private async void onTapClimatica(object sender, EventArgs e)
    {
        if (pagina != "climatica")
        {
            var faseClimatica = new FaseClimatica(_usuario);// Crea una instancia de la página FaseClimatica
            await Navigation.PushAsync(faseClimatica, false);// Navega hacia la página FaseClimatica sin animación
        }
    }
    // Método para manejar el evento de toque en la sección hidráulica del menú
    private async void onTapHidraulica(object sender, EventArgs e)
    {
        if (pagina != "hidraulica")
        {
            var faseHidraulica = new FaseHidraulica(_usuario);// Crea una instancia de la página FaseHidraulica
            await Navigation.PushAsync(faseHidraulica);// Navega hacia la página FaseHidraulica
        }
    }
    // Método para manejar el evento de toque en la sección laboratorio del menú
    private async void onTapLaboratorio(object sender, EventArgs e)
    {
        if (pagina != "laboratorio")
        {
            var faseLaboratorio = new FaseLaboratorio(_usuario);// Crea una instancia de la página FaseLaboratorio
            await Navigation.PushAsync(faseLaboratorio);// Navega hacia la página FaseLaboratorio
        }

    }
    // Método para manejar el evento de toque en la sección lumínica del menú
    private async void onTapLuminica(object sender, EventArgs e)
    {
        if (pagina != "luminica")
        {
            var faseLuminica = new FaseLuminica( _usuario);// Crea una instancia de la página FaseLuminica
            await Navigation.PushAsync(faseLuminica);// Navega hacia la página FaseLuminica
        }
    }

    // Método para manejar el evento de toque en la sección red del menú
    private async void onTapRed(object sender, EventArgs e)
    {
        if (pagina != "red")
        {
            var faseRed = new FaseRed(_usuario);// Crea una instancia de la página FaseRed
            await Navigation.PushAsync(faseRed);// Navega hacia la página FaseRed
        }
    }

    // Método para manejar el evento de toque en la sección administrador del menú
    private async void onTapAdministrador(object sender, EventArgs e)
    {
        if (pagina != "administrador")
        {
            var administrador = new Administrador(_usuario);// Crea una instancia de la página Administrador

            await Navigation.PushAsync(administrador);// Navega hacia la página Administrador
        }
        
    }
    // Método para mostrar el menú lateral y establecer la página actual
    public void Show(String page)
    {
        this.IsVisible = true; // Muestra el menú lateral
        pagina = page;// Establece la página actual
    }

    // Método para ocultar el menú lateral y establecer la página actual
    public void UnShow(String page)
    {
        this.IsVisible = false; // Oculta el menú lateral
        pagina = page;// Establece la página actual
    }

    // Método para manejar el evento de clic en el botón de cerrar sesión
    private void OnClickCerrarSesion(object sender, EventArgs e)
    {
        
        var inicioSesion = new NavigationPage(new InicioSesion());// Crea una nueva instancia de la página de inicio de sesión
        Application.Current.MainPage = inicioSesion;// Establece la página principal de la aplicación como la página de inicio de sesión
    }

    
}