using RaspiRemote.Enums;
using RaspiRemote.Models;

namespace RaspiRemote.Parsers
{
    public static class RaspiGpioParser
    {
        /// <summary>
        /// Parse information about all usable GPIO pins
        /// </summary>
        /// <param name="textToParse">A response from raspi-gpio command</param>
        /// <returns>A list with GPIO pins infos</returns>
        public static List<GpioPinInfo> ParseAllGpioPinsInfo(string textToParse)
        {
            var output = new List<GpioPinInfo>();
            var pinList = textToParse.Split("\n").Skip(1).Take(28); // usable GPIO pins have numbers from 0 to 27

            foreach (var pin in pinList)
            {
                output.Add(ParseGpioPinInfo(pin));
            }

            return output;
        }

        /// <summary>
        /// Parse information about single GPIO pin
        /// </summary>
        /// <param name="textToParse">A response from raspi-gpio command</param>
        /// <returns>A GPIO pin info</returns>
        public static GpioPinInfo ParseGpioPinInfo(string textToParse)
        {
            textToParse = textToParse.Trim();
            var output = new GpioPinInfo();
            output.Pin = GetPinNumber(textToParse);
            output.State = GetState(textToParse);
            output.Function = GetPinFunction(textToParse);
            output.Pull = GetPinPull(textToParse);
            return output;
        }

        private static GpioPin GetPinNumber(string str)
        {
            var pinNumber = int.Parse(str[5..str.IndexOf(": ")]);
            return (GpioPin)pinNumber;
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
