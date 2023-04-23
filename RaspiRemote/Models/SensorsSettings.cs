using RaspiRemote.Enums;

namespace RaspiRemote.Models
{
    class SensorsSettings
    {
        public DHT11Settings DHT11Settings { get; set; }
        public DS18B20Settings DS18B20Settings { get; set; }
    }

    class DHT11Settings
    {
        public SensorState State { get; set; }
        public GpioPin Pin { get; set; }
    }

    class DS18B20Settings
    {
        public SensorState State { get; set; }
    }
}
