namespace MauiAppStart
{
    public partial class MainPage : ContentPage
    {
        int count = 0;
        private readonly IDeviceInfoService _deviceInfoService;
        public MainPage(IDeviceInfoService deviceInfoService)
        {
            InitializeComponent();
            _deviceInfoService = deviceInfoService;
            lblPiattaformaEsecuzione.Text = _deviceInfoService.GetDeviceModel();
        }

        private void OnCounterClicked(object sender, EventArgs e)
        {
            count++;

            if (count == 1)
                CounterBtn.Text = $"Clicked {count} time";
            else
                CounterBtn.Text = $"Clicked {count} times";

            SemanticScreenReader.Announce(CounterBtn.Text);
        }
    }

}
