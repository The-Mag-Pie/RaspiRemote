using RaspiRemote.Enums;

namespace RaspiRemote.Models
{
    public class GpioPinInfo
    {
        public GpioPin GpioPin { get; set; }

        public int GpioNumber
        {
            get => (int)GpioPin;
            set => GpioPin = (GpioPin)value;
        }

        public bool State { get; set; } // True when state = 1, False when state = 0

        public GpioPinFunction Function { get; set; }

        public GpioPinPull Pull { get; set; }
    }
}
