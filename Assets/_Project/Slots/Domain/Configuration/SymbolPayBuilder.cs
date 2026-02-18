using Project.Slots.Domain.Configuration.Definitions;
using Project.Slots.Domain.Symbols;
using System.Collections.Generic;
using UnityEngine;

namespace Project.Slots.Domain.Configuration
{
    public class SymbolPayBuilder
    {
        private readonly SymbolType Symbol;
        private readonly Dictionary<int, int> Payouts = new();

        public SymbolPayBuilder(SymbolType symbol)
        {
            Symbol = symbol;
        }

        public SymbolPayBuilder AddPayout(int count, int payout)
        {
            if (Payouts.ContainsKey(count))
            {
                throw new System.Exception($"Duplicate payout for {Symbol} with {count}.");
            }

            Payouts[count] = payout;
            return this;
        }

        public SymbolPayDefinition Build()
        {
            return new SymbolPayDefinition(Symbol, Payouts);
        }
    }
}