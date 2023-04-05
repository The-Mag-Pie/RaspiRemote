using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using RaspiRemote.Popups;

namespace RaspiRemote.ViewModels
{
    internal partial class BaseViewModel : ObservableObject
    {
        [ObservableProperty]
        private bool _isBusy = false;

        protected async Task InvokeAsyncWithLoadingPopup(Func<Task> action)
        {
            IsBusy = true;
            var loadingPopup = new LoadingPopup();
            Application.Current.MainPage.ShowPopup(loadingPopup);

            await action.Invoke();

            loadingPopup.Close();
            IsBusy = false;
        }
    }
}
