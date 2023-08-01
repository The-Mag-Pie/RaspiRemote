using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Mvvm.Input;
using RaspiRemote.Enums;
using RaspiRemote.Models;
using RaspiRemote.Parsers;
using Renci.SshNet;

namespace RaspiRemote.ViewModels.Gpio
{
    internal partial class GpioPinViewModel : BaseViewModel
    {
        public static List<GpioPinFunction> GpioPinFunctions => Enum.GetValues<GpioPinFunction>().ToList();
        public static List<GpioPinPull> GpioPinPullStates => Enum.GetValues<GpioPinPull>().ToList();

        private GpioPinInfo _pinInfo;
        private readonly SshClient _sshClient;

        public int PinNumber => (int)_pinInfo.Pin;

        public int State => _pinInfo.State ? 1 : 0;

        public GpioPinFunction Function
        {
            get => _pinInfo.Function;
            set
            {
                if (value != _pinInfo.Function)
                {
                    _ = InvokeAsyncWithLoader(async () => await ChangePinFunction(value));
                }
            }
        }

        public GpioPinPull Pull
        {
            get => _pinInfo.Pull;
            set
            {
                if (value != _pinInfo.Pull)
                {
                    _ = InvokeAsyncWithLoader(async () => await ChangePinPull(value));
                }
            }
        }

        // Pull state can be changed only when pin is set to input
        public bool IsPullChangeAvailable => Function == GpioPinFunction.Input;

        public GpioPinViewModel(GpioPinInfo pinInfo, SshClient sshClient)
        {
            _pinInfo = pinInfo;
            _sshClient = sshClient;
        }

        private async Task ChangePinFunction(GpioPinFunction newValue)
        {
            string cmd;
            switch (newValue)
            {
                case GpioPinFunction.Input:
                    cmd = RaspiGpioCommands.SetInputCmd(PinNumber);
                    break;

                case GpioPinFunction.Output:
                    cmd = RaspiGpioCommands.SetOutputCmd(PinNumber);
                    break;

                default:
                    return;
            }

            await HandleRunSshCommand(cmd, "Pin function has been successfully changed.");
        }

        private async Task ChangePinPull(GpioPinPull newValue)
        {
            string cmd;
            switch (newValue)
            {
                case GpioPinPull.Up:
                    cmd = RaspiGpioCommands.SetPullUpCmd(PinNumber);
                    break;

                case GpioPinPull.Down:
                    cmd = RaspiGpioCommands.SetPullDownCmd(PinNumber);
                    break;

                case GpioPinPull.None:
                    cmd = RaspiGpioCommands.SetPullNoneCmd(PinNumber);
                    break;

                default:
                    return;
            }

            await HandleRunSshCommand(cmd, "Pin pull state has been successfully changed.");
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
                RaspiGpioCommands.SetLowStateCmd(PinNumber) :
                RaspiGpioCommands.SetHighStateCmd(PinNumber);

            await HandleRunSshCommand(cmdStr, "Pin state has been successfully changed.");
        }

        private async Task<bool> HandleRunSshCommand(string command, string successMessage)
        {
            try
            {
                var cmd = await RunCommandAsync(command);

                if (cmd.Result == string.Empty)
                {
                    _ = Toast.Make(successMessage);
                    await RefreshData();
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

        private async Task RefreshData()
        {
            var cmdStr = RaspiGpioCommands.GetSinglePinCmd(PinNumber);
            var cmd = await RunCommandAsync(cmdStr);
            _pinInfo = RaspiGpioParser.ParseGpioPinInfo(cmd.Result);

            OnPropertyChanged(nameof(State));
            OnPropertyChanged(nameof(Function));
            OnPropertyChanged(nameof(IsPullChangeAvailable));
            OnPropertyChanged(nameof(Pull));
        }

        private async Task<SshCommand> RunCommandAsync(string command)
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
            return cmd;
        }
    }
}
