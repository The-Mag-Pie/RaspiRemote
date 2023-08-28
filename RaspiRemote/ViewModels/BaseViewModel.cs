using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Maui;
using RaspiRemote.Popups;

namespace RaspiRemote.ViewModels
{
    public partial class BaseViewModel : ObservableObject
    {
        private static LoadingPopup _loadingPopup = null;
        private static bool IsLoaderVisible
        {
            set
            {
                if (_loadingPopup is not null && value is false)
                {
                    _loadingPopup.Close();
                    _loadingPopup = null;   
                }
                else if (_loadingPopup is null && value is true)
                {
                    _loadingPopup = new LoadingPopup();
                    ShowPopup(_loadingPopup);
                }
            }
        }

        private static void ShowPopup(Popup popup) => MainThread.BeginInvokeOnMainThread(() =>
            Application.Current.MainPage.ShowPopup(popup));

        [ObservableProperty]
        private bool _isBusy = false;

        /// <summary>
        /// Show the loader on the screen and invoke action
        /// </summary>
        /// <param name="action">An action to be invoked</param>
        protected async Task InvokeAsyncWithLoader(Func<Task> action)
        {
            IsBusy = true;
            IsLoaderVisible = true;

            await action.Invoke();

            IsLoaderVisible = false;
            IsBusy = false;
        }

        /// <inheritdoc cref="InvokeAsyncWithLoader(Func{Task})"/>
        /// <remarks>
        /// IMPORTANT: This overload uses <see cref="Task.Run(Action)"/> to invoke an action
        /// so remember to use <see cref="IDispatcher.Dispatch(Action)"/> in order to make changes in UI.
        /// </remarks>
        protected async Task InvokeAsyncWithLoader(Action action) =>
            await InvokeAsyncWithLoader(async () => await Task.Run(action));

        /// <inheritdoc cref="Page.DisplayAlert(string, string, string)"/>
        protected static async Task DisplayAlert(string title, string message, string cancel) =>
            await Application.Current.MainPage.DisplayAlert(title, message, cancel);

        /// <inheritdoc cref="Page.DisplayAlert(string, string, string, string)"/>
        protected static async Task<bool> DisplayAlert(string title, string message, string accept, string cancel) =>
            await Application.Current.MainPage.DisplayAlert(title, message, accept, cancel);

        /// <inheritdoc cref="Page.DisplayPromptAsync(string, string, string, string, string, int, Keyboard, string)"/>
        protected static async Task<string> DisplayPromptAsync(string title, string message, string accept = "OK", string cancel = "Cancel", string placeholder = null, int maxLength = -1, Keyboard keyboard = null, string initialValue = "") =>
            await Application.Current.MainPage.DisplayPromptAsync(title, message, accept, cancel, placeholder, maxLength, keyboard, initialValue);

        /// <inheritdoc cref="Page.DisplayActionSheet(string, string, string, string[])"/>
        protected static async Task<string> DisplayActionSheet(string title, string cancel, string destruction, params string[] buttons) =>
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
        protected static async Task<string> DisplayMenuPopup(string title, string cancel, params string[] options) =>
            await DisplayActionSheet(title, cancel, null, options);

        /// <summary>
        /// Displays an alert with error message.
        /// </summary>
        /// <param name="message">Message to be displayed</param>
        protected static async Task DisplayError(string message) => await MainThread.InvokeOnMainThreadAsync(async () =>
            await DisplayAlert("Error", message, "OK"));
    }
}
