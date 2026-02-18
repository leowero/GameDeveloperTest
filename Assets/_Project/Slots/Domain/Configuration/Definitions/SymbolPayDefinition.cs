using Project.Slots.Domain.Symbols;
using System.Collections.Generic;

namespace Project.Slots.Domain.Configuration.Definitions
{
    public class SymbolPayDefinition
    {
        public SymbolType SymbolType { get; }
        private readonly Dictionary<int, int> Payouts;

        public SymbolPayDefinition(SymbolType symbolType, Dictionary<int, int> payouts)
        {
            SymbolType = symbolType;
            Payouts = payouts;
        }

        public bool TryGetPayout(int count, out int payout)
        {
            return Payouts.TryGetValue(count, out payout);
        }
    }
}