using RaspiRemote.Enums;

namespace RaspiRemote.ViewModels
{
    class SensorsPageViewModel : BaseViewModel
    {
        public List<SensorState> SensorStates { get; } = Enum.GetValues<SensorState>().Cast<SensorState>().ToList();
        public List<GpioPin> GpioPins { get; } = Enum.GetValues<GpioPin>().Cast<GpioPin>().ToList();

        private SshClientContainer _sshClientContainer;

        public SensorsPageViewModel(SshClientContainer sshClientContainer)
        {
            _sshClientContainer = sshClientContainer;
        }
    }
}
