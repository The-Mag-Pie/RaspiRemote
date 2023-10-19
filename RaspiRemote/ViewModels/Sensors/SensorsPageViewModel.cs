using CommunityToolkit.Mvvm.Input;
using RaspiRemote.Enums;
using RaspiRemote.LocalAppData;
using RaspiRemote.Models;
using Renci.SshNet;
using System.Collections.ObjectModel;

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
    }
}
