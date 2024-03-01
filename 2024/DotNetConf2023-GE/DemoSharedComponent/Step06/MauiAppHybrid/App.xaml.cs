using RazorCLib;

namespace MauiAppHybrid
{
    public partial class App : Application
    {
        public App(ApplicationState applicationState)
        {
            InitializeComponent();

            MainPage = new MainPage(applicationState);
        }
    }
}
