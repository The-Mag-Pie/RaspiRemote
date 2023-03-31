using CommunityToolkit.Mvvm.Input;
using RaspiRemote.Models;
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
        private void AddDevice()
        {
            Devices.Add(new()
            {
                Name = "nazwa 1",
                Host = "192.168.1.1",
                Username = "użytkownik",
                Password = "hasło"
            });

            SaveDevices();
        }

        [RelayCommand]
        private void ConnectToDevice(RpiDevice device)
        {
            Application.Current.MainPage.DisplayAlert("Connect", $"Connect clicked: {device.Name}", "OK");
        }

        [RelayCommand]
        private void OpenDeviceProperties(RpiDevice device)
        {
            Application.Current.MainPage.DisplayAlert("Properties", $"Properties clicked: {device.Name}", "OK");
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
