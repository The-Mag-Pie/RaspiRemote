using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using RaspiRemote.Popups;

namespace RaspiRemote.ViewModels
{
    internal partial class BaseViewModel : ObservableObject
    {
        [ObservableProperty]
        private bool _isBusy = false;

        /// <summary>
        /// Show the loader on the screen and invoke action
        /// </summary>
        /// <param name="action">An action to be invoked</param>
        protected async Task InvokeAsyncWithLoader(Func<Task> action)
        {
            IsBusy = true;
            var loadingPopup = new LoadingPopup();
            Application.Current.MainPage.ShowPopup(loadingPopup);

            await action.Invoke();

            loadingPopup.Close();
            IsBusy = false;
        }

        /// <inheritdoc cref="Page.DisplayAlert(string, string, string)"/>
        protected async Task DisplayAlert(string title, string message, string cancel) =>
            await Application.Current.MainPage.DisplayAlert(title, message, cancel);

        /// <inheritdoc cref="Page.DisplayAlert(string, string, string, string)"/>
        protected async Task<bool> DisplayAlert(string title, string message, string accept, string cancel) =>
            await Application.Current.MainPage.DisplayAlert(title, message, accept, cancel);

        /// <inheritdoc cref="Page.DisplayPromptAsync(string, string, string, string, string, int, Keyboard, string)"/>
        protected async Task<string> DisplayPromptAsync(string title, string message, string accept = "OK", string cancel = "Cancel", string placeholder = null, int maxLength = -1, Keyboard keyboard = null, string initialValue = "") =>
            await Application.Current.MainPage.DisplayPromptAsync(title, message, accept, cancel, placeholder, maxLength, keyboard, initialValue);
    }
}
