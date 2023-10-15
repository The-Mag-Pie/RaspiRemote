using CommunityToolkit.Mvvm.ComponentModel;
using RaspiRemote.Enums;
using RaspiRemote.LocalAppData;
using RaspiRemote.Models;
using Renci.SshNet;

namespace RaspiRemote.ViewModels
{
    internal partial class SensorsPageViewModel : BaseViewModel
    {
        public List<SensorState> SensorStates { get; } = Enum.GetValues<SensorState>().ToList();
        public List<GpioPin> GpioPins { get; } = Enum.GetValues<GpioPin>().ToList();

        private SensorState _dht11SensorState;
        public SensorState DHT11SensorState
        {
            get { return _dht11SensorState; }
            set
            {
                _dht11SensorState = value;
                SaveSettings();
                OnPropertyChanged(nameof(DHT11SensorState));
                OnPropertyChanged(nameof(IsDHT11SensorEnabled));
            }
        }
        public bool IsDHT11SensorEnabled => DHT11SensorState == SensorState.ON;

        private GpioPin _dht11SensorPin;
        public GpioPin DHT11SensorPin
        {
            get { return _dht11SensorPin; }
            set
            {
                _dht11SensorPin = value;
                SaveSettings();
                OnPropertyChanged(nameof(DHT11SensorPin));
            }
        }

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(DHT11SensorTempLabelColor))]
        private string _DHT11SensorTemperature = "--";
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(DHT11SensorHumidityLabelColor))]
        private string _DHT11SensorHumidity = "--";

        public Color DHT11SensorTempLabelColor
        {
            get
            {
                if (DHT11SensorTemperature == "--")
                    return Colors.Gray;

                var temp = double.Parse(DHT11SensorTemperature, System.Globalization.NumberStyles.AllowDecimalPoint,
                    System.Globalization.NumberFormatInfo.InvariantInfo);
                if (temp < 10.0)
                    return Colors.DarkBlue;
                else if (temp < 15)
                    return Colors.Blue;
                else if (temp < 30)
                    return Colors.Orange;
                else if (temp < 40)
                    return Colors.Red;
                else
                    return Colors.DarkRed;
            }
        }

        public Color DHT11SensorHumidityLabelColor
        {
            get
            {
                if (DHT11SensorHumidity == "--")
                    return Colors.Gray;
                else
                    return Color.FromArgb("#007DA5");
            }
        }

        private SensorState _ds18b20SensorState;
        public SensorState DS18B20SensorState
        {
            get { return _ds18b20SensorState; }
            set
            {
                _ds18b20SensorState = value;
                SaveSettings();
                OnPropertyChanged(nameof(DS18B20SensorState));
            }
        }

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(DS18B20SensorTempLabelColor))]
        private string _DS18B20SensorTemperature = "--";

        public Color DS18B20SensorTempLabelColor
        {
            get
            {
                if (DS18B20SensorTemperature == "--")
                    return Colors.Gray;

                var temp = double.Parse(DS18B20SensorTemperature);
                if (temp < 10.0)
                    return Colors.DarkBlue;
                else if (temp < 15)
                    return Colors.Blue;
                else if (temp < 30)
                    return Colors.Orange;
                else if (temp < 40)
                    return Colors.Red;
                else
                    return Colors.DarkRed;
            }
        }

        private RpiDevice _deviceInfo;
        private SshClient _sshClient;
        private SftpClient _sftpClient;

        private bool _isInitialized = false;

        private Thread _dht11UpdateThread;
        private Thread _ds18b20UpdateThread;

        public SensorsPageViewModel(SshClientContainer sshClientContainer)
        {
            _deviceInfo = sshClientContainer.DeviceInfo;
            _sshClient = sshClientContainer.SshClient;
            _sftpClient = sshClientContainer.SftpClient;

            sshClientContainer.Disconnecting += DeleteExecutable;

            LoadSettings();
        }

        public async Task OnAppearing()
        {
            if (_isInitialized is false)
            {
                await UploadExecutable();
                _isInitialized = true;
            }

            //StartUpdateThreads();
        }

        public void OnDisappearing()
        {
            //StopUpdateThreads();
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

        private void DeleteExecutable()
        {
            var dirPath = "~/raspiremote";
            _sshClient.RunCommand($"rm -r {dirPath}");
        }

        private void StartUpdateThreads()
        {
            _dht11UpdateThread = new Thread(DHT11UpdateData);
            _dht11UpdateThread.Start();
            //_ds18b20UpdateThread = new Thread(DS18B20UpdateData);
            //_ds18b20UpdateThread.Start();
        }

        private void StopUpdateThreads()
        {
            _dht11UpdateThread.Interrupt();
            //_ds18b20UpdateThread.Interrupt();
        }

        private void DHT11UpdateData()
        {
            while (true)
            {
                try
                {
                    var sshCommand = _sshClient.RunCommand($"~/raspiremote_test/ReadSensorData dht11 {(int)DHT11SensorPin}");
                    var result = sshCommand.Result;

                    if (result.Length == 0 || result.Contains("ERROR"))
                        throw new Exception();

                    var data = result.Split('\n');
                    //DHT11SensorTemperature = double.Parse(data[0], System.Globalization.NumberStyles.AllowDecimalPoint,
                    //    System.Globalization.NumberFormatInfo.InvariantInfo);
                    //DHT11SensorHumidity = int.Parse(data[1]);
                    DHT11SensorTemperature = data[0];
                    DHT11SensorHumidity = data[1];

                    Thread.Sleep(3 * 1000);
                }
                catch (ThreadInterruptedException)
                {
                    DHT11SensorTemperature = "--";
                    DHT11SensorHumidity = "--";
                    return;
                }
                catch (Exception)
                {
                    DHT11SensorTemperature = "--";
                    DHT11SensorHumidity = "--";
                }
            }
        }

        private void DS18B20UpdateData()
        {
            try
            {
                for (int i = 0; i < 50; i++)
                {
                    DS18B20SensorTemperature = (-1.00 + i).ToString();
                    Thread.Sleep(3 * 100);
                }
            }
            catch (ThreadInterruptedException)
            {
                return;
            }
        }

        private void LoadSettings()
        {
            var deviceGuid = _deviceInfo.DeviceGUID;
            var settings = SensorsSettingsAppData.GetSensorsSettings(deviceGuid);

            if (settings.DHT11Settings != null)
            {
                _dht11SensorState = settings.DHT11Settings.State;
                _dht11SensorPin = settings.DHT11Settings.Pin;
            }

            if (settings.DS18B20Settings != null)
            {
                _ds18b20SensorState = settings.DS18B20Settings.State;
            }
        }

        private void SaveSettings()
        {
            var settings = new SensorsSettings()
            {
                DHT11Settings = new()
                {
                    State = DHT11SensorState,
                    Pin = DHT11SensorPin
                },
                DS18B20Settings = new()
                {
                    State = DS18B20SensorState
                }
            };

            var deviceGuid = _deviceInfo.DeviceGUID;
            SensorsSettingsAppData.SaveSensorsSettings(deviceGuid, settings);
        }
    }
}
