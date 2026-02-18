using Project.Slots.Domain.Configuration.Definitions;
using Project.Slots.Domain.Reels;
using Project.Slots.Domain.Symbols;
using UnityEngine;

namespace Project.Slots.Domain.Engine
{
    public static class SlotMachineEngine
    {
        public static SymbolType[][] Spin()
        {
            SymbolType[][] grid = new SymbolType[SlotDefinition.Columns][];

            for (int i = 0; i < grid.Length; i++)
            {
                grid[i] = new SymbolType[SlotDefinition.Rows];
            }

            for (int column = 0; column < SlotDefinition.Columns; column++)
            {
                int reelLength = ReelStrips.Reels[column].Length;
                int startIndex = Random.Range(0, reelLength);

                string selectedSymbols = CircularSubstring(ReelStrips.Reels[column], startIndex, SlotDefinition.Rows);

                for (int row = 0; row < SlotDefinition.Rows; row++)
                {
                    grid[column][row] = SymbolsConstants.SymbolsMapping[selectedSymbols[row]];
                }
            }

            return grid;
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