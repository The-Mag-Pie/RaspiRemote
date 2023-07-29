using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Mvvm.Input;
using RaspiRemote.Enums;
using RaspiRemote.Models;
using Renci.SshNet;

namespace RaspiRemote.ViewModels.Gpio
{
    internal partial class GpioPinViewModel : BaseViewModel
    {
        public static List<GpioPinFunction> GpioPinFunctions => Enum.GetValues<GpioPinFunction>().ToList();
        public static List<GpioPinPull> GpioPinPullStates => Enum.GetValues<GpioPinPull>().ToList();

        private readonly GpioPinInfo _pinInfo;
        private readonly SshClient _sshClient;

        public int PinNumber => (int)_pinInfo.Pin;

        public int State => _pinInfo.State ? 1 : 0;

        public GpioPinFunction Function
        {
            get => _pinInfo.Function;
            set
            {
                ChangePinFunction(value);
                OnPropertyChanged(nameof(Function));
                OnPropertyChanged(nameof(IsPullChangeAvailable));
            }
        }

        public GpioPinPull Pull
        {
            get => _pinInfo.Pull;
            set
            {
                ChangePinPull(value);
                OnPropertyChanged(nameof(Pull));
            }
        }

        // Pull state can be changed only when pin is set to input
        public bool IsPullChangeAvailable => Function == GpioPinFunction.Input;

        public GpioPinViewModel(GpioPinInfo pinInfo, SshClient sshClient)
        {
            _pinInfo = pinInfo;
            _sshClient = sshClient;
        }

        private void ChangePinFunction(GpioPinFunction newValue)
        {
            _pinInfo.Function = newValue;
        }

        private void ChangePinPull(GpioPinPull newValue)
        {
            _pinInfo.Pull = newValue;
        }

        [RelayCommand]
        private async Task ToggleState()
        {
            // Pin state cannot be changed if pin function = input
            if (_pinInfo.Function == GpioPinFunction.Input)
            {
                _ = DisplayError("State can be changed only when GPIO pin function is set to Output");
                return;
            }

            await InvokeAsyncWithLoader(HandleToggleState);
        }

        private async Task HandleToggleState()
        {
            var cmdStr = _pinInfo.State ? 
                RaspiGpioCommands.SetLowStateCmd((int)_pinInfo.Pin) :
                RaspiGpioCommands.SetHighStateCmd((int)_pinInfo.Pin);

            if (await HandleRunSshCommand(cmdStr, "Pin state has been changed successfully."))
            {
                _pinInfo.State = !_pinInfo.State;
                OnPropertyChanged(nameof(State));
            }
        }

        private async Task<bool> HandleRunSshCommand(string command, string successMessage)
        {
            try
            {
                var cmd = await Task.Run(() => _sshClient.RunCommand(command));
                if (cmd.ExitStatus != 0 && cmd.Error == string.Empty)
                {
                    throw new Renci.SshNet.Common.SshException(cmd.Result);
                }
                else if (cmd.ExitStatus != 0)
                {
                    throw new Renci.SshNet.Common.SshException(cmd.Error);
                }

                if (cmd.Result == string.Empty)
                {
                    _ = Toast.Make(successMessage);
                }
                else
                {
                    throw new Renci.SshNet.Common.SshException(cmd.Result);
                }
                return true;
            }
            catch (Exception ex)
            {
                _ = DisplayError(ex.Message);
                return false;
            }
        }
    }
}
