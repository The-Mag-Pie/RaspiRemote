namespace RaspiRemote.Parsers
{
    public static class FnKeyCodeParser
    {
        private static readonly Dictionary<int, string> FnKeyCodes = new()
        {
            { 1, "\x1BOP" },
            { 2, "\x1BOQ" },
            { 3, "\x1BOR" },
            { 4, "\x1BOS" },
            { 5, "\x1B[15~" },
            { 6, "\x1B[17~" },
            { 7, "\x1B[18~" },
            { 8, "\x1B[19~" },
            { 9, "\x1B[20~" },
            { 10, "\x1B[21~" },
            { 11, "\x1B[23~" },
            { 12, "\x1B[24~" }
        };

        /// <summary>
        /// Get a Fn key code assigned to the number.
        /// </summary>
        /// <param name="number">A Fn key number.</param>
        /// <returns>A Fn key code</returns>
        /// <exception cref="ArgumentException"></exception>
        public static string GetFnKeyCode(string number)
        {
            var success = int.TryParse(number, out var fnKeyNumber);

            if (success is false)
                throw new ArgumentException("Specified input is not a number.");

            if (FnKeyCodes.ContainsKey(fnKeyNumber) is false)
                throw new ArgumentException("Specified input is not a valid Fn key number (should be between 1 and 12).");

            return FnKeyCodes[fnKeyNumber];
        }
    }
}
