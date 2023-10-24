using CommunityToolkit.Mvvm.ComponentModel;
using System.Globalization;

namespace RaspiRemote.ViewModels.Sensors
{
    public partial class DS18B20SensorViewModel : SensorViewModelBase
    {
        [ObservableProperty]
        private string _sensorID;

        [ObservableProperty]
        private string _temperature = "--";

        public DS18B20SensorViewModel(string sensorID)
        {
            SensorID = sensorID;
            StartUpdating();
        }

        protected override void Update()
        {
            var cmd = _sshClient.CreateCommand($"~/raspiremote/ReadSensorData ds18b20 {SensorID}");

            while (_ct.IsCancellationRequested is false)
            {
                cmd.Execute();
                var result = cmd.Result.Trim();

                if (cmd.ExitStatus == 0 && result.Length > 1 &&
                    double.TryParse(result, NumberStyles.Any, CultureInfo.InvariantCulture, out var resultDouble))
                {
                    Temperature = Math.Round(resultDouble, 2).ToString();
                }

                Thread.Sleep(5 * 1000);
            }
        }
    }
}
