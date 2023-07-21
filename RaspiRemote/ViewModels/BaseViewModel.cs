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

        /// <inheritdoc cref="Page.DisplayActionSheet(string, string, string, string[])"/>
        protected async Task<string> DisplayActionSheet(string title, string cancel, string destruction, params string[] buttons) =>
            await Application.Current.MainPage.DisplayActionSheet(title, cancel, destruction, buttons);

        /// <summary>
        /// Displays a menu with specified options.
        /// </summary>
        /// <remarks>
        /// This method simply displays an action sheet with destroy button set to null.
        /// </remarks>
        /// <param name="title">Title of the menu</param>
        /// <param name="cancel">Text on cancel button</param>
        /// <param name="options">Menu options</param>
        /// <returns>A string with selected option</returns>
        protected async Task<string> DisplayMenuPopup(string title, string cancel, params string[] options) =>
            await DisplayActionSheet(title, cancel, null, options);
    }
}
