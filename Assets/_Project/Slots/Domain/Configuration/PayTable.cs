using Project.Slots.Domain.Configuration.Definitions;
using Project.Slots.Domain.Symbols;
using System.Collections.Generic;

namespace Project.Slots.Domain.Configuration
{
    public class PayTable
    {
        private readonly Dictionary<SymbolType, SymbolPayDefinition> _Definitions;

        public PayTable(Dictionary<SymbolType, SymbolPayDefinition> definitions)
        {
            _Definitions = definitions;
        }

        public bool TryGetPayout(SymbolType symbol, int count, out int payout)
        {
            payout = 0;

            if (!_Definitions.TryGetValue(symbol, out var def))
            {
                return false;
            }

            return def.TryGetPayout(count, out payout);
        }
    }
}