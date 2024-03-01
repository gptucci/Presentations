using RazorCLib;

namespace MauiAppHybrid;

public partial class MainPage : ContentPage
{
    public MainPage(ApplicationState applicationState)
    {
        InitializeComponent();


        applicationState.OnChanged += StateChenged;
        this.applicationState = applicationState;
    }

    private readonly ApplicationState applicationState;

    private void StateChenged()
    {
        lblValoreAttualeCounter.Text= $"Il valore attuale del contatore è {applicationState.CurrentCount.ToString()} - Da pagina MAUI";
    }

   
}
