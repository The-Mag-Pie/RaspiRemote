using Iot.Device.DHTxx;
using Iot.Device.OneWire;
using System.Text.Json;

namespace ReadSensorData
{
    internal class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 2)
            {
                Console.WriteLine("ERROR: Too few parameters.");
                Environment.Exit(27);
                return;
            }

            if (args.Length > 2)
            {
                Console.WriteLine("ERROR: Too many parameters.");
                Environment.Exit(27);
                return;
            }

            switch (args[0])
            {
                case "dht11":
                    handleDHT11DataRequest(args[1]);
                    break;
                case "ds18b20":
                    handleDS18B20DataRequest(args[1]);
                    break;
                default:
                    Console.WriteLine($"ERROR: Unrecognized parameter: {args[0]}");
                    Environment.Exit(27);
                    break;
            }
        }

        static void handleDHT11DataRequest(string arg)
        {
            int pin;
            try
            {
                pin = int.Parse(arg);
            }
            catch
            {
                Console.WriteLine($"ERROR: Wrong parameter: pin = {arg}");
                Environment.Exit(27);
                return;
            }

            using var dht11 = new Dht11(pin);

            bool success;
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
        }

        static void handleDS18B20DataRequest(string arg)
        {
            var devices = OneWireThermometerDevice.EnumerateDevices()
                .Where(d => d.Family == DeviceFamily.Ds18b20);

            try
            {
                if (arg == "list")
                {
                    var devicesIds = devices.Select(d => d.DeviceId);
                    Console.WriteLine(JsonSerializer.Serialize(devicesIds));
                    return;
                }

                var device = devices.Where(d => d.DeviceId == arg).Single();
                Console.WriteLine(device.ReadTemperature().DegreesCelsius);
            }
            catch (DirectoryNotFoundException)
            {
                Console.WriteLine("OneWire interface is disabled in Raspberry Pi settings.");
                Environment.Exit(25);
            }
            catch (InvalidOperationException)
            {
                Console.WriteLine("OneWire device with specified ID was not found.");
                Environment.Exit(26);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Environment.Exit(27);
            }
        }
    }
}