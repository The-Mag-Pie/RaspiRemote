﻿using RaspiRemote.Models;
using RaspiRemote.Parsers;
using Renci.SshNet;
using System.Collections.ObjectModel;

namespace RaspiRemote.ViewModels.Gpio
{
    internal partial class GpioPageViewModel : BaseViewModel
    {
        private readonly SshClient _sshClient;
        private readonly RpiDevice _deviceInfo;

        public ObservableCollection<GpioPinViewModel> GpioPins { get; set; } = new();

        public GpioPageViewModel(SshClientContainer sshClientContainer)
        {
            _sshClient = sshClientContainer.SshClient;
            _deviceInfo = sshClientContainer.DeviceInfo;
        }

        public async Task OnAppearing()
        {
            await LoadDataWithLoader();
        }

        // TODO: loader crashes an app
        private async Task LoadDataWithLoader() => await InvokeAsyncWithLoader(LoadData);

        private async Task LoadData()
        {
            var command = _sshClient.RunCommand(RaspiGpioCommands.GetAllPins);
            if (command.ExitStatus != 0)
            {
                await DisplayAlert("Error", command.Error, "OK");
                return;
            }

            var result = command.Result;
            if (result.Contains("Must be root"))
            {
                await AddUserToGroup();
            }
            else
            {

                var pins = RaspiGpioParser.ParseAllGpioPinsInfo(result);
                await Task.Run(() => AddPinsToCollection(pins));
            }
        }

        private void AddPinsToCollection(List<GpioPinInfo> pins)
        {
            //GpioPins.Clear(); // bug in windows: after calling Clear() items in collection are duplicated
            foreach (var item in GpioPins.ToList())
            {
                GpioPins.Remove(item);
            }

            foreach (var pin in pins)
            {
                GpioPins.Add(new(pin, _sshClient));
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
