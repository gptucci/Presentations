using Blazing.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Shared.Mvvm;

public partial class SecondaPaginaViewModel : ViewModelBase
{
    [ObservableProperty]

    bool _isBusy;

    [ObservableProperty]
    string _messaggio = string.Empty;


    public override async Task OnInitializedAsync()
    {
        IsBusy = true;

        Messaggio = "Pagina 2";
        IsBusy = false;
        await base.OnInitializedAsync();

    }
}
