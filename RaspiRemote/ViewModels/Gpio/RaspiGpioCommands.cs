namespace RaspiRemote.ViewModels.Gpio
{
    public readonly struct RaspiGpioCommands
    {
        private static readonly string BaseCommand = "raspi-gpio";
        private static readonly string GetSinglePin = BaseCommand + " get {pin}";
        private static readonly string SetInput = BaseCommand + " set {pin} ip";
        private static readonly string SetOutput = BaseCommand + " set {pin} op";
        private static readonly string SetPullUp = BaseCommand + " set {pin} pu";
        private static readonly string SetPullDown = BaseCommand + " set {pin} pd";
        private static readonly string SetPullNone = BaseCommand + " set {pin} pn";
        private static readonly string SetHighState = BaseCommand + " set {pin} dh";
        private static readonly string SetLowState = BaseCommand + " set {pin} dl";

        public static readonly string GetAllPins = BaseCommand + " get";

        public static string GetSinglePinCmd(int pinNumber) => GetSinglePin.Replace("{pin}", pinNumber.ToString());
        public static string SetInputCmd(int pinNumber) => SetInput.Replace("{pin}", pinNumber.ToString());
        public static string SetOutputCmd(int pinNumber) => SetOutput.Replace("{pin}", pinNumber.ToString());
        public static string SetPullUpCmd(int pinNumber) => SetPullUp.Replace("{pin}", pinNumber.ToString());
        public static string SetPullDownCmd(int pinNumber) => SetPullDown.Replace("{pin}", pinNumber.ToString());
        public static string SetPullNoneCmd(int pinNumber) => SetPullNone.Replace("{pin}", pinNumber.ToString());
        public static string SetHighStateCmd(int pinNumber) => SetHighState.Replace("{pin}", pinNumber.ToString());
        public static string SetLowStateCmd(int pinNumber) => SetLowState.Replace("{pin}", pinNumber.ToString());
    }
}
