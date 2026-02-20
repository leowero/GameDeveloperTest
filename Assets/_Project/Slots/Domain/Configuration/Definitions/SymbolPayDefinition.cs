using Project.Slots.Domain.Symbols;
using System.Collections.Generic;

namespace Project.Slots.Domain.Configuration.Definitions
{
    public class SymbolPayDefinition
    {
        private readonly SymbolType _SymbolType;
        private readonly Dictionary<int, int> _Payouts;
        public SymbolType SymbolType => _SymbolType;

        public SymbolPayDefinition(SymbolType symbolType, Dictionary<int, int> payouts)
        {
            _SymbolType = symbolType;
            _Payouts = payouts;
        }

        public bool TryGetPayout(int count, out int payout)
        {
            return _Payouts.TryGetValue(count, out payout);
        }
    }
}