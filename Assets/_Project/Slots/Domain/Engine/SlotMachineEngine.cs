using Project.Slots.Data;
using Project.Slots.Domain.Configuration.Definitions;
using Project.Slots.Domain.Reels;
using Project.Slots.Domain.Symbols;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Project.Slots.Domain.Engine
{
    /// <summary>
    /// Static slot-machine core engine.
    /// </summary>
    /// <remarks>
    /// Responsibilities include:
    /// - Generating a spin outcome grid based on reel strips and stop indexes,
    /// - Compiling winning lines by evaluating the grid against configured patterns,
    /// - Providing small helper utilities used by the engine.
    ///
    /// This implementation relies on static configuration sources such as
    /// <see cref="SlotDefinition"/>, <see cref="ReelStrips"/>, <see cref="SymbolsConstants"/>,
    /// and <see cref="PayTableData"/>, which makes it simple to call but tightly coupled.
    /// </remarks>
    public static class SlotMachineEngine
    {
        /// <summary>
        /// Performs a spin and returns the resulting symbol grid.
        /// </summary>
        /// <param name="stopIndexes">
        /// Output array with the final stop index used for each column.
        /// </param>
        /// <param name="forcedStopIndexes">
        /// Optional stop indexes to force a deterministic spin outcome.
        /// When provided, its length must match <see cref="SlotDefinition.Columns"/>.
        /// </param>
        /// <returns>
        /// A 2D jagged array representing the grid in <c>[column][row]</c> order.
        /// </returns>
        /// <remarks>
        /// Each column uses its reel strip from <see cref="ReelStrips"/> and selects a contiguous
        /// segment of <see cref="SlotDefinition.Rows"/> symbols starting at the chosen stop index.
        /// Stop indexes are normalized using modulo arithmetic to support negative or overflow values.
        ///
        /// Random stops use <see cref="Random.Range(int, int)"/>.
        /// </remarks>
        /// <exception cref="ArgumentException">
        /// Thrown when <paramref name="forcedStopIndexes"/> is provided but its length does not match the
        /// configured number of columns.
        /// </exception>
        public static SymbolType[][] Spin(out int[] stopIndexes, int[] forcedStopIndexes = null)
        {
            if (forcedStopIndexes != null && forcedStopIndexes.Length != SlotDefinition.Columns)
            {
                throw new ArgumentException(
                    $"Expected {SlotDefinition.Columns} forced stop indexes, got {forcedStopIndexes.Length}.",
                    nameof(forcedStopIndexes));
            }

            SymbolType[][] grid = new SymbolType[SlotDefinition.Columns][];
            stopIndexes = new int[SlotDefinition.Columns];

            for (int i = 0; i < grid.Length; i++)
            {
                grid[i] = new SymbolType[SlotDefinition.Rows];
            }

            for (int column = 0; column < SlotDefinition.Columns; column++)
            {
                int reelLength = ReelStrips.Reels[column].Length;

                int stopIndex = forcedStopIndexes != null
                    ? forcedStopIndexes[column]
                    : UnityEngine.Random.Range(0, reelLength);

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

        /// <summary>
        /// Evaluates the provided grid against a set of patterns and returns the winning lines.
        /// </summary>
        /// <param name="grid">Spin outcome grid in <c>[column][row]</c> order.</param>
        /// <param name="patterns">Patterns used to determine which positions are evaluated per column.</param>
        /// <returns>
        /// A list of <see cref="WinLineDefinition"/> entries for each pattern that produces a non-zero payout.
        /// </returns>
        /// <remarks>
        /// Pattern format contract:
        /// <list type="bullet">
        /// <item><description><see cref="Pattern.pattern"/> is a comma-separated string with one segment per column</description></item>
        /// <item><description>Each segment is a '0'/'1' string whose length equals <see cref="SlotDefinition.Rows"/></description></item>
        /// <item><description>Each segment must contain exactly one '1', its index is interpreted as the row to evaluate for that column</description></item>
        /// </list>
        ///
        /// This method is resilient to malformed patterns. Invalid patterns are skipped rather than throwing,
        /// which prevents content issues from crashing gameplay in production builds.
        /// </remarks>
        public static IReadOnlyList<WinLineDefinition> CompileWins(SymbolType[][] grid, IReadOnlyList<Pattern> patterns)
        {
            List<WinLineDefinition> wins = new List<WinLineDefinition>();

            if (grid == null || grid.Length == 0 || patterns == null || patterns.Count == 0)
            {
                return wins;
            }

            int columns = SlotDefinition.Columns;
            int rows = SlotDefinition.Rows;

            foreach (Pattern pattern in patterns)
            {
                if (pattern == null || string.IsNullOrWhiteSpace(pattern.pattern))
                {
                    continue;
                }

                string[] colPatterns = pattern.pattern.Split(',');
                if (colPatterns.Length != columns)
                {
                    continue;
                }

                SymbolType firstSymbol = null;
                int matchCount = 0;

                for (int column = 0; column < columns; column++)
                {
                    if (grid[column] == null || grid[column].Length < rows)
                    {
                        matchCount = 0;
                        break;
                    }

                    if (!TryGetPatternRowIndex(colPatterns[column], rows, out int row))
                    {
                        matchCount = 0;
                        break;
                    }

                    SymbolType current = grid[column][row];

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

                if (firstSymbol == null || matchCount <= 0)
                {
                    continue;
                }

                PayTableData.PayTable.TryGetPayout(firstSymbol.Type, matchCount, out int payout);

                if (payout == 0)
                {
                    continue;
                }

                wins.Add(new WinLineDefinition(pattern.id, matchCount, firstSymbol.Type, payout));
            }

            return wins;
        }

        /// <summary>
        /// Parses a single column segment from a pattern definition and outputs the row index to evaluate.
        /// </summary>
        /// <param name="segment">
        /// Column descriptor composed of '0' and '1' characters.
        /// </param>
        /// <param name="expectedRows">
        /// Expected segment length, typically equal to <see cref="SlotDefinition.Rows"/>.
        /// </param>
        /// <param name="rowIndex">
        /// Output row index (0-based) corresponding to the position of the single '1' in the segment.
        /// </param>
        /// <returns>
        /// <c>true</c> when the segment is valid and a row index could be determined, otherwise <c>false</c>.
        /// </returns>
        /// <remarks>
        /// A segment is considered valid when:
        /// - Its length equals <paramref name="expectedRows"/>,
        /// - It contains exactly one '1',
        /// - All characters are either '0' or '1'.
        /// </remarks>
        private static bool TryGetPatternRowIndex(string segment, int expectedRows, out int rowIndex)
        {
            rowIndex = -1;

            if (string.IsNullOrEmpty(segment) || segment.Length != expectedRows)
            {
                return false;
            }

            int firstOne = -1;
            int onesCount = 0;

            for (int i = 0; i < segment.Length; i++)
            {
                char c = segment[i];

                if (c == '1')
                {
                    if (firstOne < 0)
                    {
                        firstOne = i;
                    }

                    onesCount++;
                    if (onesCount > 1)
                    {
                        return false;
                    }
                }
                else if (c != '0')
                {
                    return false;
                }
            }

            if (onesCount != 1)
            {
                return false;
            }

            rowIndex = firstOne;
            return true;
        }

        /// <summary>
        /// Returns a substring of the given string that wraps around circularly when reaching the end.
        /// </summary>
        /// <param name="input">Source string.</param>
        /// <param name="startIndex">
        /// Start index in the source string. Values are normalized, so negative or overflow indexes are supported.
        /// </param>
        /// <param name="length">Requested length of the returned string.</param>
        /// <returns>
        /// A string of the requested length produced by wrapping around the input.
        /// Returns <see cref="string.Empty"/> when <paramref name="input"/> is null or empty, or when <paramref name="length"/> is not positive.
        /// </returns>
        /// <remarks>
        /// This is used to extract a contiguous window from a reel strip, treating the reel as circular.
        /// </remarks>
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