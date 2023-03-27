using CommunityToolkit.Mvvm.ComponentModel;

namespace RaspiRemote.ViewModels
{
    internal partial class BaseViewModel : ObservableObject
    {
        [ObservableProperty]
        private bool _isBusy = false;


    }
}
