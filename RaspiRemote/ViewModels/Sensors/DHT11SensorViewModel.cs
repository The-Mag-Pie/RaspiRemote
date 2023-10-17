using CommunityToolkit.Mvvm.ComponentModel;
using RaspiRemote.Enums;
using Renci.SshNet;

namespace RaspiRemote.ViewModels.Sensors
{
    public partial class DHT11SensorViewModel : ObservableObject
    {
        private readonly SshClient _sshClient;

        [ObservableProperty]
        private GpioPin _pin;

        [ObservableProperty]
        private string _temperature = "--";

        [ObservableProperty]
        private string _humidity = "--";

        public DHT11SensorViewModel(GpioPin pin)
        {
            _sshClient = ServiceHelper.GetService<SshClientContainer>().SshClient;
            Pin = pin;
            StartUpdating();
        }

        public DHT11SensorViewModel(int pin) : this((GpioPin)pin) { }

        public void StartUpdating()
        {
            var cmd = _sshClient.CreateCommand($"~/raspiremote/ReadSensorData dht11 {(int)Pin}");
            Task.Run(() =>
            {
                cmd.Execute();

                var result = cmd.Result.Trim().Split("\n");
                if (cmd.ExitStatus == 0 && result.Length == 2)
                {
                    Temperature = result[0];
                    Humidity = result[1];
                } 
            });
        }

        public void StopUpdating()
        {

        }
    }
}
