using CommunityToolkit.Mvvm.ComponentModel;
using RaspiRemote.Enums;
using Renci.SshNet;
using System.Diagnostics;

namespace RaspiRemote.ViewModels.Sensors
{
    public partial class DHT11SensorViewModel : ObservableObject
    {
        private readonly SshClient _sshClient;
        private CancellationTokenSource _ctSource;
        private CancellationToken _ct;
        private bool _ctSourceDisposed = false;

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
            StopUpdating();

            _ctSource = new CancellationTokenSource();
            _ct = _ctSource.Token;
            _ctSourceDisposed = false;

            Task.Run(Update, _ct);
        }

        public void StopUpdating()
        {
            if (_ctSource is not null && _ctSourceDisposed is false)
            {
                _ctSource.Cancel();
                _ctSource.Dispose();
                _ctSourceDisposed = true;
            }
        }

        private void Update()
        {
            var cmd = _sshClient.CreateCommand($"~/raspiremote/ReadSensorData dht11 {(int)Pin}");

            while (_ct.IsCancellationRequested is false)
            {
                cmd.Execute();

                var result = cmd.Result.Trim().Split("\n");
                if (cmd.ExitStatus == 0 && result.Length == 2)
                {
                    Temperature = result[0];
                    Humidity = result[1];
                }

                Thread.Sleep(5 * 1000);
            }

            _ct.ThrowIfCancellationRequested();
        }
    }
}
