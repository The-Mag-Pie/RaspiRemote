namespace RaspiRemote.Parsers
{
    public static class CtrlCharacterParser
    {
        private static readonly List<char> CtrlChars = new()
        {
            '@', 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I',
            'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S',
            'T', 'U', 'V', 'W', 'X', 'Y', 'Z', '[', '\\', ']',
            '^', '_'
        };

        /// <summary>
        /// Get a Ctrl character assigned to the passed character.
        /// </summary>
        /// <param name="text">A character to parse.</param>
        /// <returns>A Ctrl character</returns>
        /// <exception cref="ArgumentException"></exception>
        public static char GetCtrlCharacter(string text)
        {
            if (text.Length != 1)
                throw new ArgumentException("Specified input is empty or have more than one character.");

            var chr = text.ToUpper()[0];
            var idx = CtrlChars.IndexOf(chr);

            if (idx == -1)
                throw new ArgumentException("Specified input is not a valid Ctrl character.");

            return (char)idx;

        }
    }
}
