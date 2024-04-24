using Shared.Mvvm;

namespace MauiGenoaExpenseMng
{
    public partial class MainPage : ContentPage
    {
        int count = 0;

        public MainPage()
        {
            InitializeComponent();
            this.BindingContext = new MainPageViewModel();
        }

        protected override async void OnAppearing()
        {
            MainPageViewModel vm = this.BindingContext as MainPageViewModel;
            await vm.InitializeAsync();
            base.OnAppearing();
        }
    }

}
