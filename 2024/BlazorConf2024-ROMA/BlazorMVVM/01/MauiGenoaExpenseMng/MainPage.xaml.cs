using MauiGenoaExpenseMng.ViewModel;

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
        
    }

}
