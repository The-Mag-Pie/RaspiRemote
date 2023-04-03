using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.Input;
using RaspiRemote.Models;
using RaspiRemote.Popups;
using System.Collections.ObjectModel;

namespace RaspiRemote.ViewModels
{
    internal partial class SelectDevicePageViewModel : BaseViewModel
    {
        public ObservableCollection<RpiDevice> Devices { get; } = new();

        public SelectDevicePageViewModel()
        {
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

        [RelayCommand]
        private void DeleteDevice(RpiDevice device)
        {
            Devices.Remove(device);
            SaveDevices();
        }

        [RelayCommand]
        private async Task ConnectToDevice(RpiDevice device)
        {
            //Application.Current.MainPage.DisplayAlert("Connect", $"Connect clicked: {device.Name}", "OK");
            await EditDevice(device);
        }

        [RelayCommand]
        private async Task OpenDeviceOptions(RpiDevice device)
        {
            var popup = new DeviceOptionsPopup();
            var result = await Application.Current.MainPage.ShowPopupAsync(popup);
            if (result != null)
            {
                switch (result)
                {
                    case DeviceOptionsActions.Edit:
                        _ = EditDevice(device);
                        break;

                    case DeviceOptionsActions.Delete:
                        DeleteDevice(device);
                        break;
                }
            }
        }

        private void LoadDevices()
        {
            Devices.Clear();

            foreach (var device in LocalAppData.GetDevicesList())
            {
                Devices.Add(device);
            }
        }

        private void SaveDevices()
        {
            LocalAppData.SaveDevicesList(Devices.ToList());
        }
    }
}
