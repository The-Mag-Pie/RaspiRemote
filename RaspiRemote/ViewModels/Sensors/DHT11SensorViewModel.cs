﻿using CommunityToolkit.Mvvm.ComponentModel;
using RaspiRemote.Enums;
using System.Diagnostics;

namespace RaspiRemote.ViewModels.Sensors
{
    public partial class DHT11SensorViewModel : SensorViewModelBase
    {
        [ObservableProperty]
        private GpioPin _pin;

        [ObservableProperty]
        private string _temperature = "--";

        [ObservableProperty]
        private string _humidity = "--";

        public DHT11SensorViewModel(GpioPin pin)
        {
            Pin = pin;
            StartUpdating();
        }

        public DHT11SensorViewModel(int pin) : this((GpioPin)pin) { }

        protected override void Update()
        {
            var cmd = _sshClient.CreateCommand($"~/raspiremote/ReadSensorData dht11 {(int)Pin}");

            while (true)
            {
                _ct.ThrowIfCancellationRequested();

                cmd.Execute();
                var result = cmd.Result.Trim().Split("\n");

                if (cmd.ExitStatus == 0 && result.Length == 2)
                {
                    Temperature = result[0];
                    Humidity = result[1];
                }

                Thread.Sleep(5 * 1000);
            }
        }
    }
}
