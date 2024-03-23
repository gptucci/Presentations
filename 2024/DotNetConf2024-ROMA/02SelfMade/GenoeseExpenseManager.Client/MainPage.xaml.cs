namespace GenoeseExpenseManager.Client;

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
        var mRemoteDBService = ServiceProvider.Current.GetService<IRemoteDBService>();
        await mRemoteDBService.InitializeAsync();
        await vm.InitializzaVmAsync();
    }
}
