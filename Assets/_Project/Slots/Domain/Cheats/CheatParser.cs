using System;

namespace Project.Slots.Domain.Cheats
{
    /// <summary>
    /// Parses user-facing cheat commands into <see cref="CheatRequest"/> instances.
    /// </summary>
    /// <remarks>
    /// Supported formats:
    /// - "C:3" forces a win for symbol 'C' with match count 3 on any satisfiable pattern.
    /// - "C:3:0" forces a win for symbol 'C' with match count 3 on pattern id 0.
    ///
    /// This parser validates syntax and numeric formats. Semantic validation
    /// (e.g. symbol existence, payout rules, satisfiable patterns) is handled by the cheat planner.
    /// </remarks>
    public static class CheatParser
    {
        /// <summary>
        /// Attempts to parse a cheat command string.
        /// </summary>
        /// <param name="input">Raw input string.</param>
        /// <param name="request">Parsed request when successful, otherwise <c>null</c>.</param>
        /// <param name="error">Error description when parsing fails, otherwise <c>null</c>.</param>
        /// <returns><c>true</c> when parsing succeeds, otherwise <c>false</c>.</returns>
        public static bool TryParse(string input, out CheatRequest request, out string error)
        {
            request = null;
            error = null;

            if (string.IsNullOrWhiteSpace(input))
            {
                error = "Empty input.";
                return false;
            }

            string trimmed = input.Trim();
            string[] parts = trimmed.Split(':');

            if (parts.Length != 2 && parts.Length != 3)
            {
                error = "Invalid format. Use \"C:3\" or \"C:3:0\".";
                return false;
            }

            if (parts[0].Length != 1)
            {
                error = "Symbol must be exactly 1 character, for example \"C\".";
                return false;
            }

            char symbol = parts[0][0];

            if (!int.TryParse(parts[1], out int matchCount))
            {
                error = "Invalid match count.";
                return false;
            }

            if (matchCount <= 0)
            {
                error = "Match count must be greater than zero.";
                return false;
            }

            if (parts.Length == 2)
            {
                request = CheatRequest.AnyPattern(symbol, matchCount);
                return true;
            }

            if (!int.TryParse(parts[2], out int patternId))
            {
                error = "Invalid pattern id.";
                return false;
            }

            if (patternId < 0)
            {
                error = "Pattern id must be non-negative.";
                return false;
            }

            request = CheatRequest.SpecificPattern(symbol, matchCount, patternId);
            return true;
        }
    }
}