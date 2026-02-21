using Project.Slots.Data;
using Project.Slots.Domain.Configuration.Definitions;
using Project.Slots.Domain.Reels;
using System.Collections.Generic;
using System.Linq;

namespace Project.Slots.Domain.Cheats
{
    /// <summary>
    /// Builds forced reel stop indexes that aim to satisfy a cheat request.
    /// </summary>
    /// <remarks>
    /// The planner attempts to choose stop indexes such that the visible grid contains
    /// a requested symbol on the requested number of consecutive columns following a pattern.
    ///
    /// The planner is best-effort. It returns <c>false</c> when:
    /// - The symbol is unknown,
    /// - The requested match count has no payout configured,
    /// - The requested pattern cannot be satisfied given the current reel strips.
    /// </remarks>
    public static class CheatPlanner
    {
        /// <summary>
        /// Attempts to translate a <see cref="CheatRequest"/> into forced stop indexes.
        /// </summary>
        /// <param name="request">Cheat request to satisfy.</param>
        /// <param name="patterns">Available pattern definitions.</param>
        /// <param name="stopIndexes">Forced stop indexes when successful, otherwise <c>null</c>.</param>
        /// <returns><c>true</c> when a forced stop plan could be built, otherwise <c>false</c>.</returns>
        public static bool TryBuildForcedStops(CheatRequest request, IReadOnlyList<Pattern> patterns, out int[] stopIndexes)
        {
            stopIndexes = null;

            if (request == null)
            {
                return false;
            }

            if (patterns == null || patterns.Count == 0)
            {
                return false;
            }

            int columns = ReelStrips.Reels.Count;

            if (columns <= 0)
            {
                return false;
            }

            if (request.MatchCount <= 0 || request.MatchCount > columns)
            {
                return false;
            }

            // Validate that the requested symbol and match count are meaningful in the configured pay table.
            // This prevents forcing outcomes that are "wins" visually but pay zero.
            if (!PayTableData.PayTable.TryGetPayout(request.SymbolId, request.MatchCount, out int payout) || payout <= 0)
            {
                return false;
            }

            if (request.Kind == CheatKind.ForceWinSpecificPattern)
            {
                if (!request.PatternId.HasValue)
                {
                    return false;
                }

                Pattern pattern = patterns.FirstOrDefault(x => x != null && x.id == request.PatternId.Value);
                if (pattern == null)
                {
                    return false;
                }

                return TryBuildForPattern(pattern.pattern, request.SymbolId, request.MatchCount, columns, out stopIndexes);
            }

            foreach (Pattern pattern in patterns)
            {
                if (pattern == null)
                {
                    continue;
                }

                if (TryBuildForPattern(pattern.pattern, request.SymbolId, request.MatchCount, columns, out stopIndexes))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Attempts to build forced stop indexes for a specific pattern string.
        /// </summary>
        private static bool TryBuildForPattern(string patternString, char symbolId, int matchCount, int columns, out int[] stopIndexes)
        {
            stopIndexes = null;

            if (string.IsNullOrWhiteSpace(patternString))
            {
                return false;
            }

            string[] colPatterns = patternString.Split(',');
            if (colPatterns.Length != columns)
            {
                return false;
            }

            stopIndexes = new int[columns];

            for (int column = 0; column < matchCount; column++)
            {
                string colPattern = colPatterns[column];

                if (string.IsNullOrEmpty(colPattern))
                {
                    return false;
                }

                int row = colPattern.IndexOf('1');
                if (row < 0 || row >= SlotDefinition.Rows)
                {
                    return false;
                }

                string strip = ReelStrips.Reels[column];
                int chosen = FindAnyStopIndexFor(strip, symbolId, row);
                if (chosen < 0)
                {
                    return false;
                }

                stopIndexes[column] = Mod(chosen, strip.Length);
            }

            // Remaining columns are not forced to a specific outcome beyond index 0.
            // This keeps the plan simple but may result in additional unintended matches.
            for (int column = matchCount; column < stopIndexes.Length; column++)
            {
                stopIndexes[column] = 0;
            }

            return true;
        }

        /// <summary>
        /// Finds any stop index that will place the desired symbol at the specified visible row.
        /// </summary>
        /// <param name="strip">Reel strip definition.</param>
        /// <param name="symbolId">Desired symbol identifier.</param>
        /// <param name="row">Target visible row.</param>
        /// <returns>
        /// A stop index that satisfies the request, or -1 when no position can satisfy it.
        /// </returns>
        private static int FindAnyStopIndexFor(string strip, char symbolId, int row)
        {
            if (string.IsNullOrEmpty(strip))
            {
                return -1;
            }

            int length = strip.Length;

            for (int position = 0; position < length; position++)
            {
                if (strip[position] != symbolId)
                {
                    continue;
                }

                int stopIndex = position - row;
                return Mod(stopIndex, length);
            }

            return -1;
        }

        /// <summary>
        /// Positive modulo helper.
        /// </summary>
        private static int Mod(int x, int m)
        {
            int r = x % m;
            if (r < 0)
            {
                return r + m;
            }

            return r;
        }
    }
}