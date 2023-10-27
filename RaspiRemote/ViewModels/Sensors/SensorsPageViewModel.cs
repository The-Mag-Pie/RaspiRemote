using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RaspiRemote.Enums;
using RaspiRemote.LocalAppData;
using RaspiRemote.Models;
using Renci.SshNet;
using System.Collections.ObjectModel;
using System.Text.Json;

namespace RaspiRemote.ViewModels.Sensors
{
    internal partial class SensorsPageViewModel : BaseViewModel
    {
        private static readonly IEnumerable<string> GpioPins = Enum.GetValues<GpioPin>().Select(v => v.ToString());

        private RpiDevice _deviceInfo;
        private SshClient _sshClient;
        private SftpClient _sftpClient;

        private event Action Appearing;
        private event Action Disappearing;

        [ObservableProperty]
        private bool _isRefreshing;

        public ObservableCollection<DHT11SensorViewModel> DHT11Sensors { get; } = new();
        public ObservableCollection<DS18B20SensorViewModel> DS18B20Sensors { get; } = new();

        public SensorsPageViewModel(SshClientContainer sshClientContainer)
        {
            _deviceInfo = sshClientContainer.DeviceInfo;
            _sshClient = sshClientContainer.SshClient;
            _sftpClient = sshClientContainer.SftpClient;

            sshClientContainer.Disconnecting += RemoveExecutable;
            sshClientContainer.Disconnecting += OnDisappearing;

            _ = Initialize();
        }

        public async Task Initialize() => await InvokeAsyncWithLoader(async () =>
        {
            await UploadExecutable();
            LoadDS18B20Sensors();
            LoadDHT11Sensors();
        });

        public void OnAppearing()
        {
            Appearing?.Invoke();
        }

        public void OnDisappearing()
        {

            Disappearing?.Invoke();
        }

        private async Task UploadExecutable() => await InvokeAsyncWithLoader(async () =>
        {
            try
            {
                var dirPath = "~/raspiremote";
                var filePath = dirPath + "/ReadSensorData";

                _sshClient.RunCommand($"mkdir {dirPath}");
                filePath = _sshClient.RunCommand($"echo {filePath}").Result.Trim(); // ~ to home directory path conversion

                var bit = _sshClient.RunCommand("getconf LONG_BIT").Result.Trim();
                using var fileStream = await FileSystem.OpenAppPackageFileAsync($"sensors{bit}/ReadSensorData");

                _sftpClient.UploadFile(fileStream, filePath, true);
                _sshClient.RunCommand($"chmod +x {filePath}");
            }
            catch (Exception ex)
            {
                await DisplayError(ex.Message);
            }
        });

        private void RemoveExecutable()
        {
            var dirPath = "~/raspiremote";
            _sshClient.RunCommand($"rm -r {dirPath}");
        }

        private void LoadDS18B20Sensors()
        {
            var cmd = _sshClient.RunCommand("~/raspiremote/ReadSensorData ds18b20 list");
            var result = cmd.Result.Trim();

            if (cmd.ExitStatus == 0 && result.Length > 2)
            {
                var sensorIds = JsonSerializer.Deserialize<List<string>>(result);
                if (sensorIds is null) return;

                foreach (var sensorId in sensorIds)
                {
                    var sensor = new DS18B20SensorViewModel(sensorId);
                    Appearing += sensor.StartUpdating;
                    Disappearing += sensor.StopUpdating;

                    DS18B20Sensors.Add(sensor);
                }
            }
            else if (cmd.ExitStatus == 25)
            {
                _ = DisplayAlert("Error",
                    "OneWire interface must be enabled in order to view available DS18B20 sensors. Enable it using raspi-config command.",
                    "OK");
            }
        }

        private void LoadDHT11Sensors()
        {
            var sensorsList = SensorsAppData.GetDHT11SensorsList(_deviceInfo.DeviceGUID);
            foreach (var sensorPin in sensorsList)
            {
                var sensor = new DHT11SensorViewModel(sensorPin);
                Appearing += sensor.StartUpdating;
                Disappearing += sensor.StopUpdating;

                DHT11Sensors.Add(sensor);
            }
        }

        private void SaveDHT11Sensors()
        {
            var sensorsList = DHT11Sensors.Select(s => (int)s.Pin).ToList();
            SensorsAppData.SaveDHT11SensorsList(_deviceInfo.DeviceGUID, sensorsList);
        }

        [RelayCommand]
        private async Task AddDHT11Sensor()
        {
            var response = await DisplayActionSheet("Select DHT11 sensor pin", "Cancel", null,
                GpioPins.Where(v => DHT11Sensors.Any(s => s.Pin.ToString() == v) is false).ToArray());
            if (response == null || response == "Cancel") return;

            if (Enum.TryParse(response, out GpioPin sensorPin) is false)
            {
                await DisplayError("Error: wrong GPIO pin.");
                return;
            }

            var sensor = new DHT11SensorViewModel(sensorPin);
            Appearing += sensor.StartUpdating;
            Disappearing += sensor.StopUpdating;

            DHT11Sensors.Add(sensor);
            SaveDHT11Sensors();
        }

        [RelayCommand]
        private void DeleteDHT11Sensor(DHT11SensorViewModel sensor)
        {
            sensor.StopUpdating();
            DHT11Sensors.Remove(sensor);
            SaveDHT11Sensors();
        }

        [RelayCommand]
        private async Task RefreshBtn() => await InvokeAsyncWithLoader(Reload);

        [RelayCommand]
        private async Task Refresh() => await Task.Run(() =>
        {
            IsRefreshing = true;
            Reload();
            IsRefreshing = false;
        });

        private void Reload()
        {
            OnDisappearing();
            DS18B20Sensors.Clear();
            DHT11Sensors.Clear();
            RemoveEventHandlers();

            LoadDS18B20Sensors();
            LoadDHT11Sensors();
        }

        private void RemoveEventHandlers()
        {
            var appearingDelegates = Appearing?.GetInvocationList();
            if (appearingDelegates is null) return;
            foreach (var d in appearingDelegates)
            {
                Appearing -= (Action)d;
            }

            var disappearingDelegates = Disappearing?.GetInvocationList();
            if (disappearingDelegates is null) return;
            foreach (var d in disappearingDelegates)
            {
                Disappearing -= (Action)d;
            }
        }
    }
}
