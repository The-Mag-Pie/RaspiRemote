using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.Input;
using RaspiRemote.LocalAppData;
using RaspiRemote.Models;
using RaspiRemote.Popups;
using System.Collections.ObjectModel;

namespace RaspiRemote.ViewModels
{
    internal partial class SelectDevicePageViewModel : BaseViewModel
    {
        private SshClientContainer _sshClientContainer;

        public ObservableCollection<RpiDevice> Devices { get; } = new();

        public SelectDevicePageViewModel(SshClientContainer sshClientContainer)
        {
            _sshClientContainer = sshClientContainer;
            LoadDevices();
        }

        [RelayCommand]
        private async Task AddDevice()
        {
            var popup = new AddDevicePopup();
            var newDevice = await Application.Current.MainPage.ShowPopupAsync(popup) as RpiDevice;
            if (newDevice != null)
            {
                Devices.Add(newDevice);
                SaveDevices();
            }
        }

        [RelayCommand]
        private async Task ConnectToDevice(RpiDevice device)
        {
            await InvokeAsyncWithLoader(async () =>
            {
                try
                {
                    await _sshClientContainer.SetDataAndConnectAsync(device);
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Error", ex.Message, "OK");
                    return;
                }

                Application.Current.MainPage = new AppShell();
            });
        }

        [RelayCommand]
        private async Task OpenDeviceOptions(RpiDevice device)
        {
            var result = await DisplayActionSheet("Options", "Cancel", null, new[] { "Edit", "Delete" });
            if (result is null) return;

            switch (result)
            {
                case "Edit":
                    _ = EditDevice(device);
                    break;

                case "Delete":
                    DeleteDevice(device);
                    break;
            }
        }

        private async Task EditDevice(RpiDevice device)
        {
            var popup = new EditDevicePopup(device);
            var editedDevice = await Application.Current.MainPage.ShowPopupAsync(popup) as RpiDevice;
            if (editedDevice != null)
            {
                device.Name = editedDevice.Name;
                device.Host = editedDevice.Host;
                device.Port = editedDevice.Port;
                device.Username = editedDevice.Username;
                device.Password = editedDevice.Password;
                SaveDevices();
            }
        }

        private void DeleteDevice(RpiDevice device)
        {
            Devices.Remove(device);
            SaveDevices();
        }

        private void LoadDevices()
        {
            Devices.Clear();

            foreach (var device in DevicesAppData.GetDevicesList())
            {
                Devices.Add(device);
            }
        }

        private void SaveDevices()
        {
            DevicesAppData.SaveDevicesList(Devices.ToList());
        }
    }
}
