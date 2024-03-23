
namespace AzureMobSvc.Client;

public partial class MainPage : ContentPage
{
    int count = 0;
    MainPageViewModel vm;
    public MainPage()
    {
        InitializeComponent();



        vm = ServiceProvider.Current.GetService<MainPageViewModel>();
        BindingContext = vm;

    }
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await vm.InitializzaVmAsync();
    }

}
