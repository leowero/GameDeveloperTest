using Project.Slots.Data;
using Project.Slots.Domain.Configuration;
using Project.Slots.Domain.Reels;
using Project.Slots.Domain.Symbols;
using System.Collections.Generic;
using System.Linq;

namespace Project.Slots.Domain.Cheats
{
    public static class CheatPlanner
    {
        public static bool TryBuildForcedStops(CheatRequest request, IReadOnlyList<Pattern> patterns, out int[] stopIndexes)
        {
            stopIndexes = null;

            if (!SymbolsConstants.SymbolsMapping.TryGetValue(request.SymbolId, out var symbolType))
            {
                return false;
            }

            if (!PayTableData.PayTable.TryGetPayout(symbolType, request.MatchCount, out int payout) || payout <= 0)
            {
                return false;
            }

            if (request.Kind == CheatKind.ForceWinSpecificPattern)
            {
                var pattern = patterns.FirstOrDefault(x => x.id == request.PatternId);
                if (pattern == null)
                {
                    return false;
                }

                return TryBuildForPattern(pattern.pattern, request.SymbolId, request.MatchCount, out stopIndexes);
            }

            foreach (var pattern in patterns)
            {
                if (TryBuildForPattern(pattern.pattern, request.SymbolId, request.MatchCount, out stopIndexes))
                {
                    return true;
                }
                    
            }

            return false;
        }

        private static bool TryBuildForPattern(string patternString, char symbolId, int matchCount, out int[] stopIndexes)
        {
            stopIndexes = new int[ReelStrips.Reels.Length];

            string[] colPatterns = patternString.Split(',');

            for (int column = 0; column < matchCount; column++)
            {
                int row = colPatterns[column].IndexOf('1');
                if (row < 0)
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

            for (int column = matchCount; column < stopIndexes.Length; column++)
            {
                stopIndexes[column] = 0;
            }

            return true;
        }

        private static int FindAnyStopIndexFor(string strip, char symbolId, int row)
        {
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

        private static int Mod(int x, int m)
        {
            int r = x % m;
            return r < 0 ? r + m : r;
        }
    }
}
