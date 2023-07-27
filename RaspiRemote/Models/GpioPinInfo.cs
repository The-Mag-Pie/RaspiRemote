using RaspiRemote.Enums;

namespace RaspiRemote.Models
{
    public class GpioPinInfo
    {
        /// <summary>
        /// GPIO pin number
        /// </summary>
        public GpioPin Pin { get; set; }

        /// <summary>
        /// State of the pin. True if state = 1, False if state = 0
        /// </summary>
        public bool State { get; set; }

        /// <summary>
        /// Function of the pin
        /// </summary>
        public GpioPinFunction Function { get; set; }

        /// <summary>
        /// Pull state of the pin
        /// </summary>
        public GpioPinPull Pull { get; set; }
    }
}
