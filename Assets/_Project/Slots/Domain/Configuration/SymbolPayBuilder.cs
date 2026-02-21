using Project.Slots.Domain.Configuration.Definitions;
using System;
using System.Collections.Generic;

namespace Project.Slots.Domain.Configuration
{
    /// <summary>
    /// Builder used to create a <see cref="SymbolPayDefinition"/> for a single symbol.
    /// </summary>
    public sealed class SymbolPayBuilder
    {
        private readonly char _SymbolId;
        private readonly Dictionary<int, int> _Payouts = new Dictionary<int, int>();

        /// <summary>
        /// Creates a builder for the specified symbol id.
        /// </summary>
        /// <param name="symbolId">Raw symbol identifier.</param>
        public SymbolPayBuilder(char symbolId)
        {
            _SymbolId = symbolId;
        }

        /// <summary>
        /// Adds a payout entry for a given match count.
        /// </summary>
        /// <param name="count">Match count.</param>
        /// <param name="payout">Payout value.</param>
        /// <returns>The same builder instance for fluent chaining.</returns>
        /// <exception cref="InvalidOperationException">
        /// Thrown when a payout for the same match count is added more than once.
        /// </exception>
        public SymbolPayBuilder AddPayout(int count, int payout)
        {
            if (_Payouts.ContainsKey(count))
            {
                throw new InvalidOperationException($"Duplicate payout for symbol '{_SymbolId}' with count {count}.");
            }

            _Payouts[count] = payout;
            return this;
        }

        /// <summary>
        /// Builds the <see cref="SymbolPayDefinition"/> from the currently configured payouts.
        /// </summary>
        /// <returns>A new <see cref="SymbolPayDefinition"/>.</returns>
        public SymbolPayDefinition Build()
        {
            return new SymbolPayDefinition(_SymbolId, _Payouts);
        }
    }
}