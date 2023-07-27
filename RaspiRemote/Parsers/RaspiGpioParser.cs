using RaspiRemote.Enums;
using RaspiRemote.Models;

namespace RaspiRemote.Parsers
{
    public static class RaspiGpioParser
    {
        /// <summary>
        /// Parse information about all usable GPIO pins
        /// </summary>
        /// <param name="content">A response from raspi-gpio command</param>
        /// <returns>A list with GPIO pins infos</returns>
        public static List<GpioPinInfo> ParseAllGpioPinsInfo(string content)
        {
            var output = new List<GpioPinInfo>();
            var pinList = content.Split("\n").Skip(1).Take(28);

            foreach (var pin in pinList)
            {
                output.Add(ParseGpioPinInfo(pin));
            }

            return output;
        }

        /// <summary>
        /// Parse information about single GPIO pin
        /// </summary>
        /// <param name="content">A response from raspi-gpio command</param>
        /// <returns>A GPIO pin info</returns>
        public static GpioPinInfo ParseGpioPinInfo(string content)
        {
            var output = new GpioPinInfo();
            output.GpioNumber = GetPinNumber(content);
            output.State = GetState(content);
            output.Function = GetPinFunction(content);
            output.Pull = GetPinPull(content);
            return output;
        }

        private static int GetPinNumber(string str)
        {
            return int.Parse(str[5..str.IndexOf(": ")]);
        }

        private static bool GetState(string str)
        {
            var idx = str.IndexOf("level=") + 6;
            return str[idx..(idx + 1)] == "1";
        }

        private static GpioPinFunction GetPinFunction(string str)
        {
            var startIdx = str.IndexOf("func=") + 5;
            var endIdx = str.IndexOf(" pull=");
            var funcStr = str[startIdx..endIdx];
            if (funcStr == "INPUT") return GpioPinFunction.Input;
            else if (funcStr == "OUTPUT") return GpioPinFunction.Output;
            else throw new FormatException($"Unknown GPIO function: {funcStr}");
        }

        private static GpioPinPull GetPinPull(string str)
        {
            var idx = str.IndexOf("pull=") + 5;
            var pullStr = str[idx..];
            if (pullStr == "UP") return GpioPinPull.Up;
            else if (pullStr == "DOWN") return GpioPinPull.Down;
            else if (pullStr == "NONE") return GpioPinPull.None;
            else throw new FormatException($"Unknown GPIO pull: {pullStr}");
        }
    }
}
