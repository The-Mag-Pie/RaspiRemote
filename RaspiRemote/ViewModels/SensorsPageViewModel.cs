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
        private double _DHT11SensorTemperature;
        [ObservableProperty]
        private int _DHT11SensorHumidity;

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
        private double _DS18B20SensorTemperature;

        private RpiDevice _deviceInfo;
        private SshClient _sshClient;

        public SensorsPageViewModel(SshClientContainer sshClientContainer)
        {
            _deviceInfo = sshClientContainer.DeviceInfo;
            _sshClient = sshClientContainer.SshClient;

            LoadSettings();
        }

        private Thread _dht11UpdateThread;
        private Thread _ds18b20UpdateThread;

        public void OnAppearing()
        {
            _dht11UpdateThread = new Thread(DHT11UpdateData);
            _dht11UpdateThread.Start();
            _ds18b20UpdateThread = new Thread(DS18B20UpdateData);
            _ds18b20UpdateThread.Start();
        }

        public void OnDisappearing()
        {
            _dht11UpdateThread.Interrupt();
            _ds18b20UpdateThread.Interrupt();
        }

        private void DHT11UpdateData()
        {
            while (true)
            {
                try
                {
                    var sshCommand = _sshClient.RunCommand("~/raspiremote_test/ReadSensorData dht11 4");
                    var result = sshCommand.Result;

                    if (result.Length != 0 && result.Contains("ERROR") == false)
                    {
                        var data = result.Split('\n');
                        DHT11SensorTemperature = double.Parse(data[0], System.Globalization.NumberStyles.AllowDecimalPoint,
                            System.Globalization.NumberFormatInfo.InvariantInfo);
                        DHT11SensorHumidity = int.Parse(data[1]);
                    }

                    Thread.Sleep(3 * 1000);
                }
                catch (ThreadInterruptedException)
                {
                    return;
                }
                catch (FormatException)
                {
                    return;
                }
            }
        }

        private void DS18B20UpdateData()
        {
            try
            {
                for (int i = 0; i < 50; i++)
                {
                    DS18B20SensorTemperature = 49.52 + i;
                    Thread.Sleep(3 * 1000);
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
