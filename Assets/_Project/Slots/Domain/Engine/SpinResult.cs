using Project.Slots.Domain.Configuration.Definitions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Project.Slots.Domain.Engine
{
    /// <summary>
    /// Represents the outcome of a single slot-machine spin.
    /// </summary>
    /// <remarks>
    /// A spin result aggregates:
    /// - The compiled winning lines,
    /// - The final reel stop indexes,
    /// - The total payout derived from the wins.
    ///
    /// This type is logically immutable. Input collections are copied during construction
    /// and exposed through read-only interfaces to prevent external mutation after creation.
    /// </remarks>
    public sealed class SpinResult
    {
        /// <summary>
        /// Read-only collection of winning line definitions produced by the spin.
        /// </summary>
        /// <remarks>
        /// Contains one entry per pattern that resulted in a non-zero payout.
        /// The collection is never <c>null</c>, but it may be empty.
        /// </remarks>
        public IReadOnlyList<WinLineDefinition> Wins { get; }

        /// <summary>
        /// Final stop indexes used for each reel column.
        /// </summary>
        /// <remarks>
        /// The length of this array is expected to match the configured number of columns.
        /// Values represent normalized stop positions on each reel strip.
        /// </remarks>
        public IReadOnlyList<int> StopIndexes { get; }

        /// <summary>
        /// Total payout for the spin, computed as the sum of all individual win payouts.
        /// </summary>
        /// <remarks>
        /// This value is computed eagerly from <see cref="Wins"/> at construction time.
        /// It does not include bet multipliers, bonus modifiers, or other adjustments applied elsewhere.
        /// </remarks>
        public double TotalPayout { get; }

        /// <summary>
        /// Creates a new <see cref="SpinResult"/> from the given win lines and stop indexes.
        /// </summary>
        /// <param name="wins">Winning line definitions for the spin.</param>
        /// <param name="stopIndexes">Final stop indexes used for each reel column.</param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="wins"/> or <paramref name="stopIndexes"/> is <c>null</c>.
        /// </exception>
        /// <remarks>
        /// Input collections are copied defensively to avoid external mutation after construction.
        /// </remarks>
        public SpinResult(IReadOnlyList<WinLineDefinition> wins, int[] stopIndexes)
        {
            if (wins == null)
            {
                throw new ArgumentNullException(nameof(wins));
            }

            if (stopIndexes == null)
            {
                throw new ArgumentNullException(nameof(stopIndexes));
            }

            // Defensive copies to guarantee logical immutability
            Wins = wins.ToList();
            StopIndexes = stopIndexes.ToArray();
            TotalPayout = Wins.Sum(w => w.Payout);
        }
    }
}