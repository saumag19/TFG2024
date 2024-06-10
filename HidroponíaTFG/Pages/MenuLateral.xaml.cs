using HidroponíaTFG.Models;

namespace HidroponíaTFG.Pages;

public partial class MenuLateral : ContentView
{
    String pagina = null;
    private Usuario _usuario;
    public MenuLateral( )
    {
        InitializeComponent();

    }

    public void setUsuario(Usuario usuario)
    {
        _usuario = usuario;
        btn_cerrar.Text = "Cerrar Sesión";
        if ( usuario.Type != "admin")
        {
            boton6.IsVisible = false;
        }
    }

    private async void onTapClimatica(object sender, EventArgs e)
    {
        if (pagina != "climatica")
        {
            var faseClimatica = new FaseClimatica(_usuario);
            await Navigation.PushAsync(faseClimatica, false);
        }
    }
    private async void onTapHidraulica(object sender, EventArgs e)
    {
        if (pagina != "hidraulica")
        {
            var faseHidraulica = new FaseHidraulica(_usuario);
            await Navigation.PushAsync(faseHidraulica);
        }
    }
    private async void onTapLaboratorio(object sender, EventArgs e)
    {
        if (pagina != "laboratorio")
        {
            var faseLaboratorio = new FaseLaboratorio(_usuario);
            await Navigation.PushAsync(faseLaboratorio);
        }

    }
    private async void onTapLuminica(object sender, EventArgs e)
    {
        if (pagina != "luminica")
        {
            var faseLuminica = new FaseLuminica( _usuario);
            await Navigation.PushAsync(faseLuminica);
        }
    }
    private async void onTapRed(object sender, EventArgs e)
    {
        if (pagina != "red")
        {
            var faseRed = new FaseRed(_usuario);
            await Navigation.PushAsync(faseRed);
        }
    }
    private async void onTapAdministrador(object sender, EventArgs e)
    {
        if (pagina != "administrador")
        {
            var administrador = new Administrador(_usuario);

            await Navigation.PushAsync(administrador);
        }
        
    }
    public void Show(String page)
    {
        this.IsVisible = true; // Muestra el popup
        pagina = page;
    }
    public void UnShow(String page)
    {
        this.IsVisible = false; // Muestra el popup
        pagina = page;
    }

    private void OnClickCerrarSesion(object sender, EventArgs e)
    {
        
        var faseClimatica = new NavigationPage(new InicioSesion());
        Application.Current.MainPage = faseClimatica;
    }

    
}