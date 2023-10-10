using CommunityToolkit.Mvvm.Input;
using RaspiRemote.Pages;
using RaspiRemote.ViewModels;
using System.Diagnostics;

namespace RaspiRemote;

public partial class ShellViewModel : BaseViewModel
{
    [RelayCommand]
    private async Task Power(string option) => await InvokeAsyncWithLoader(async () =>
    {
        var sshContainer = ServiceHelper.GetService<SshClientContainer>();
        var sshClient = sshContainer.SshClient;
        var deviceInfo = sshContainer.DeviceInfo;

        string command;

        switch (option)
        {
            case "Shutdown":
                command = "shutdown now";
                break;

            case "Reboot":
                command = "shutdown -r now";
                break;

            default:
                command = "echo \"Power option not specified\" && exit 1";
                break;
        }

        try
        {
            var resultCmd = sshClient.RunCommand($"echo \"{deviceInfo.Password}\" | sudo -S -k {command}");

            if (resultCmd.ExitStatus != 0 || resultCmd.Error.Length > 0)
            {
                throw new Renci.SshNet.Common.SshException(
                    resultCmd.Error.Length > 0 ?
                    resultCmd.Error : $"{resultCmd.Error}{resultCmd.Result}");
            }

            await Disconnect();
        }
        catch (Renci.SshNet.Common.SshConnectionException ex)
        {
            Debug.WriteLine(ex.Message);
            await Disconnect();
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", ex.Message, "OK");
        }
    });

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
    private async Task Disconnect() => await InvokeAsyncWithLoader(async () =>
    {
        var sshContainer = ServiceHelper.GetService<SshClientContainer>();
        await sshContainer.DisconnectFromDevice();

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
