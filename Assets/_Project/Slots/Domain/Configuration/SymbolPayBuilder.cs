using Project.Slots.Domain.Configuration.Definitions;
using Project.Slots.Domain.Symbols;
using System.Collections.Generic;
using UnityEngine;

namespace Project.Slots.Domain.Configuration
{
    public class SymbolPayBuilder
    {
        private readonly SymbolType _Symbol;
        private readonly Dictionary<int, int> _Payouts = new Dictionary<int, int>();

        public SymbolPayBuilder(SymbolType symbol)
        {
            _Symbol = symbol;
        }

        public SymbolPayBuilder AddPayout(int count, int payout)
        {
            if (_Payouts.ContainsKey(count))
            {
                throw new System.Exception($"Duplicate payout for {_Symbol} with {count}.");
            }

            _Payouts[count] = payout;
            return this;
        }

        public SymbolPayDefinition Build()
        {
            return new SymbolPayDefinition(_Symbol, _Payouts);
        }
    }
}