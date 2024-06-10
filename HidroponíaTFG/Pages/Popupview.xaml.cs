namespace HidroponíaTFG.Pages;

public partial class Popupview : ContentView
{
	public Popupview()
	{
		InitializeComponent();
	}
    void OnCloseButtonClicked(object sender, EventArgs e)
    {
        this.IsVisible = false; // Cierra el popup
    }

    public void Show()
    {
        this.IsVisible = true; // Muestra el popup
       
    }
}