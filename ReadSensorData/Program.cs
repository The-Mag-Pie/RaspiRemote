using Iot.Device.DHTxx;
using Iot.Device.OneWire;

namespace ReadSensorData
{
    internal class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 2)
            {
                Console.WriteLine("ERROR: Too few parameters.");
                return;
            }

            switch (args[0])
            {
                case "dht11":
                    int pin;
                    try
                    {
                        pin = int.Parse(args[1]);
                    }
                    catch
                    {
                        Console.WriteLine($"ERROR: Wrong parameter: pin = {args[1]}");
                        return;
                    }
                    handleDHT11DataRequest(pin);
                    break;
                case "ds18b20":
                    handleDS18B20DataRequest();
                    break;
                default:
                    Console.WriteLine($"ERROR: Unrecognized parameter: {args[0]}");
                    break;
            }
        }

        static void handleDHT11DataRequest(int pin)
        {
            using var dht11 = new Dht11(pin);

            bool success;
            //for (int _ = 0; _ < 500; _++)
            while (true)
            {
                success = dht11.TryReadTemperature(out var temperature);
                if (success == false)
                    continue;

                success = dht11.TryReadHumidity(out var humidity);
                if (success == false)
                    continue;

                Console.WriteLine(temperature.DegreesCelsius);
                Console.WriteLine(humidity.Percent);
                return;
            }

            Console.WriteLine("ERROR: cannot read data from DHT11 sensor");
        }

        static void handleDS18B20DataRequest()
        {
            // TODO
            OneWireThermometerDevice.EnumerateDevices().Select(d => d.Family == DeviceFamily.Ds18b20);
        }
    }
}