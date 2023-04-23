using RaspiRemote.Enums;
using RaspiRemote.LocalAppData;
using RaspiRemote.Models;

namespace RaspiRemote.ViewModels
{
    class SensorsPageViewModel : BaseViewModel
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

        private SshClientContainer _sshClientContainer;

        public SensorsPageViewModel(SshClientContainer sshClientContainer)
        {
            _sshClientContainer = sshClientContainer;

            LoadSettings();
        }

        public void OnAppearing()
        {
            
        }

        private void LoadSettings()
        {
            var settings = SensorsSettingsAppData.GetSensorsSettings();

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

            SensorsSettingsAppData.SaveSensorsSettings(settings);
        }
    }
}
