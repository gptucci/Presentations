using Akavache;
using System.Reactive.Linq;
namespace MauiAppAkavache.Client
{
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
        class Toaster
        {
            public string Name { get; set; }
        }
        protected override async void OnAppearing()
        {
            base.OnAppearing();
            var mRemoteDBService = ServiceProvider.Current.GetService<IClientRepository>();
            await mRemoteDBService.InitializeAsync();
            await vm.InitializeAsync();
        }
        
    }

}
