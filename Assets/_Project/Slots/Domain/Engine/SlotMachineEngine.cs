using Project.Slots.Data;
using Project.Slots.Domain.Configuration.Definitions;
using Project.Slots.Domain.Reels;
using Project.Slots.Domain.Symbols;
using System.Collections.Generic;
using UnityEngine;

namespace Project.Slots.Domain.Engine
{
    public static class SlotMachineEngine
    {
        public static SymbolType[][] Spin(out int[] stopIndexes, int[] forcedStopIndexes = null)
        {
            SymbolType[][] grid = new SymbolType[SlotDefinition.Columns][];
            stopIndexes = new int[SlotDefinition.Columns];

            for (int i = 0; i < grid.Length; i++)
            {
                grid[i] = new SymbolType[SlotDefinition.Rows];
            }
                

            for (int column = 0; column < SlotDefinition.Columns; column++)
            {
                int reelLength = ReelStrips.Reels[column].Length;
                int stopIndex = forcedStopIndexes != null ? forcedStopIndexes[column] : Random.Range(0, reelLength);

                stopIndex = ((stopIndex % reelLength) + reelLength) % reelLength;
                stopIndexes[column] = stopIndex;

                string selected = CircularSubstring(ReelStrips.Reels[column], stopIndex, SlotDefinition.Rows);
                for (int row = 0; row < SlotDefinition.Rows; row++)
                {
                    grid[column][row] = SymbolsConstants.SymbolsMapping[selected[row]];
                }
            }

            return grid;
        }

        public static IReadOnlyList<WinLineDefinition> CompileWins(SymbolType[][] grid, IReadOnlyList<Pattern> patterns)
        {
            List<WinLineDefinition> wins = new List<WinLineDefinition>();

            int columns = grid.Length;

            foreach (var pattern in patterns)
            {
                string[] colPatterns = pattern.pattern.Split(',');

                SymbolType firstSymbol = null;
                int matchCount = 0;

                for (int col = 0; col < columns; col++)
                {
                    int row = colPatterns[col].IndexOf('1');

                    if (row < 0)
                    {
                        break;
                    }

                    SymbolType current = grid[col][row];

                    if (firstSymbol == null)
                    {
                        firstSymbol = current;
                        matchCount = 1;
                        continue;
                    }

                    if (current.Type != firstSymbol.Type)
                    {
                        break;
                    }

                    matchCount++;
                }

                int payout = 0;
                PayTableData.PayTable.TryGetPayout(firstSymbol, matchCount, out payout);

                if (payout == 0)
                {
                    continue;
                }

                wins.Add(new WinLineDefinition(pattern.id, matchCount, firstSymbol.Type, payout));
            }

            return wins;
        }

        public static string CircularSubstring(string input, int startIndex, int length)
        {
            if (string.IsNullOrEmpty(input) || length <= 0)
            {
                return string.Empty;
            }

            int inputLength = input.Length;
            startIndex = ((startIndex % inputLength) + inputLength) % inputLength;
            char[] result = new char[length];

            for (int i = 0; i < length; i++)
            {
                int index = (startIndex + i) % inputLength;
                result[i] = input[index];
            }

            return new string(result);
        }
    }
}