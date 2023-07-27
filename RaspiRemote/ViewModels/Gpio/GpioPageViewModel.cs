using Microsoft.Maui.Controls;
using RaspiRemote.Models;
using RaspiRemote.Parsers;
using Renci.SshNet;

namespace RaspiRemote.ViewModels.Gpio
{
    internal partial class GpioPageViewModel : BaseViewModel
    {
        private readonly SshClient _sshClient;
        private readonly RpiDevice _deviceInfo;

        public GpioPageViewModel(SshClientContainer sshClientContainer)
        {
            _sshClient = sshClientContainer.SshClient;
            _deviceInfo = sshClientContainer.DeviceInfo;
        }

        public async Task OnAppearing()
        {
            await LoadData();
        }

        private async Task LoadData()
        {
            var command = _sshClient.RunCommand("raspi-gpio get");
            var result = command.Result;
            if (result.Contains("Must be root"))
            {
                await AddUserToGroup();
            }
            else
            {
                var list = RaspiGpioParser.ParseAllGpioPinsInfo(result);
                await DisplayAlert("xd", result, "ok");
            }
        }

        private async Task AddUserToGroup()
        {
            var command = _sshClient.RunCommand($"echo \"{_deviceInfo.Password}\" | sudo -S adduser {_deviceInfo.Username} gpio");
            if (command.ExitStatus != 0)
            {
                await DisplayAlert("Error", $"User {_deviceInfo.Username} is not in \"gpio\" group. An attempt to add the user to the group has failed. You have to manually add the user to the \"gpio\" group in order to use this module.", "OK");
                // TODO: navigate to another module
            }
            else
            {
                await DisplayAlert("Info", $"User {_deviceInfo.Username} has just been added to the \"gpio\" group. Please reopen an app again in order to use this module.", "OK");
                Application.Current.Quit();
            }
        }
    }
}
