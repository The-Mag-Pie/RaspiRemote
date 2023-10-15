using CommunityToolkit.Mvvm.ComponentModel;
using RaspiRemote.Enums;

namespace RaspiRemote.ViewModels.Sensors
{
    public partial class DHT11SensorViewModel : ObservableObject
    {
        public event Action PinChanged;

        public List<GpioPin> GpioPins { get; } = Enum.GetValues<GpioPin>().ToList();

        private GpioPin _pin = GpioPin.Gpio0;
        public GpioPin Pin
        {
            get => _pin;
            set
            {
                _pin = value;
                PinChanged?.Invoke();
                OnPropertyChanged(nameof(Pin));
            }
        }

        public DHT11SensorViewModel() { }
        public DHT11SensorViewModel(int pin)
        {
            Pin = (GpioPin)pin;
        }
    }
}
