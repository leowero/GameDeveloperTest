namespace Project.Slots.Domain.Cheats
{
    public static class CheatParser
    {
        public static bool TryParse(string input, out CheatRequest request, out string error)
        {
            request = null;
            error = null;

            if (string.IsNullOrWhiteSpace(input))
            {
                error = "Empty input.";
                return false;
            }

            string[] parts = input.Trim().Split(':');
            if (parts.Length != 2 && parts.Length != 3)
            {
                error = "Invalid format. Use \"C:3\" or \"C:3:0\" for example.";
                return false;
            }

            if (parts[0].Length != 1)
            {
                error = "Symbol must be 1 char, i.e. C.";
                return false;
            }

            char symbol = parts[0][0];

            if (!int.TryParse(parts[1], out int matchCount) || matchCount <= 0)
            {
                error = "Invalid count.";
                return false;
            }

            if (parts.Length == 2)
            {
                request = CheatRequest.AnyPattern(symbol, matchCount);
                return true;
            }

            if (!int.TryParse(parts[2], out int patternId) || patternId < 0)
            {
                error = "Invalid Pattern ID.";
                return false;
            }

            request = CheatRequest.SpecificPattern(symbol, matchCount, patternId);
            return true;
        }
    }
}
