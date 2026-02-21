using System;
using System.Collections.Generic;

namespace Project.Slots.Domain.Configuration.Definitions
{
    /// <summary>
    /// Defines the payout mapping for a single symbol.
    /// </summary>
    /// <remarks>
    /// The definition maps a match count to a payout value.
    /// Match counts are typically the number of consecutive matches from the first column.
    /// </remarks>
    public sealed class SymbolPayDefinition
    {
        private readonly Dictionary<int, int> _Payouts;

        /// <summary>
        /// Raw symbol identifier to which this payout definition applies.
        /// </summary>
        public char SymbolId { get; }

        /// <summary>
        /// Creates a new <see cref="SymbolPayDefinition"/>.
        /// </summary>
        /// <param name="symbolId">Raw symbol identifier.</param>
        /// <param name="payouts">Mapping of match count to payout.</param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="payouts"/> is <c>null</c>.
        /// </exception>
        /// <remarks>
        /// The input dictionary is copied defensively to prevent external mutation after construction.
        /// </remarks>
        public SymbolPayDefinition(char symbolId, Dictionary<int, int> payouts)
        {
            if (payouts == null)
            {
                throw new ArgumentNullException(nameof(payouts));
            }

            SymbolId = symbolId;
            _Payouts = new Dictionary<int, int>(payouts);
        }

        /// <summary>
        /// Attempts to get the payout configured for the given match count.
        /// </summary>
        /// <param name="count">Match count.</param>
        /// <param name="payout">Payout value when found.</param>
        /// <returns>
        /// <c>true</c> if a payout exists for the given match count, otherwise <c>false</c>.
        /// </returns>
        public bool TryGetPayout(int count, out int payout)
        {
            return _Payouts.TryGetValue(count, out payout);
        }
    }
}