using CommunityToolkit.Mvvm.Input;
using RaspiRemote.Pages;
using RaspiRemote.ViewModels;

namespace RaspiRemote;

public partial class ShellViewModel : BaseViewModel
{
	[RelayCommand]
	private async Task Reconnect() => await InvokeAsyncWithLoader(async () =>
	{
        var sshContainer = ServiceHelper.GetService<SshClientContainer>();

        try
        {
            await sshContainer.ReconnectToCurrentDevice();
            Application.Current.MainPage = new AppShell();
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", ex.Message, "OK");
            Application.Current.MainPage = new StartPage();
        }
    });

    [RelayCommand]
    private async Task Disconnect() => await InvokeAsyncWithLoader(() =>
    {
        var page = new StartPage();
        Application.Current.Dispatcher.Dispatch(() =>
        {
            Application.Current.MainPage = page;
        });
    });
}

public partial class AppShell : Shell
{
	public AppShell()
	{
		InitializeComponent();

        BindingContext = new ShellViewModel();
	}
}
