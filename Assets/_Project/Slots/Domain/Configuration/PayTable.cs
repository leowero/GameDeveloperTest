using Project.Slots.Domain.Configuration.Definitions;
using System;
using System.Collections.Generic;

namespace Project.Slots.Domain.Configuration
{
    /// <summary>
    /// Represents the payout table for the slot machine.
    /// </summary>
    /// <remarks>
    /// The pay table maps a symbol identifier to a payout definition, which then maps
    /// a match count to a payout amount.
    ///
    /// The table is keyed by the raw symbol identifier <c>char</c> to avoid relying on
    /// reference equality of <c>SymbolType</c> instances.
    /// </remarks>
    public sealed class PayTable
    {
        private readonly Dictionary<char, SymbolPayDefinition> _Definitions;

        /// <summary>
        /// Creates a new <see cref="PayTable"/> from the provided symbol payout definitions.
        /// </summary>
        /// <param name="definitions">
        /// Map of symbol id to its payout definition.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="definitions"/> is <c>null</c>.
        /// </exception>
        /// <remarks>
        /// The input dictionary is copied defensively to prevent external mutation after construction.
        /// </remarks>
        public PayTable(Dictionary<char, SymbolPayDefinition> definitions)
        {
            if (definitions == null)
            {
                throw new ArgumentNullException(nameof(definitions));
            }

            _Definitions = new Dictionary<char, SymbolPayDefinition>(definitions);
        }

        /// <summary>
        /// Attempts to get a payout for a given symbol id and match count.
        /// </summary>
        /// <param name="symbolId">Raw symbol identifier.</param>
        /// <param name="count">Match count.</param>
        /// <param name="payout">Payout value when found.</param>
        /// <returns>
        /// <c>true</c> if a payout exists for the provided symbol and count, otherwise <c>false</c>.
        /// </returns>
        /// <remarks>
        /// This method returns the payout as configured. It does not apply bet multipliers or bonus modifiers.
        /// </remarks>
        public bool TryGetPayout(char symbolId, int count, out int payout)
        {
            payout = 0;

            if (!_Definitions.TryGetValue(symbolId, out SymbolPayDefinition def))
            {
                return false;
            }

            return def.TryGetPayout(count, out payout);
        }
    }
}