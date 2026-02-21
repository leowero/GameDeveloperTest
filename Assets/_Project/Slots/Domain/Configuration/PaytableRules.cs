using Project.Slots.Data;
using Project.Slots.Domain.Symbols;

namespace Project.Slots.Domain.Configuration
{
    public static class PaytableRules
    {
        public static bool IsValidMatchCount(char symbolId, int matchCount)
        {
            if (!SymbolsConstants.SymbolsMapping.TryGetValue(symbolId, out var symbolType))
            {
                return false;
            }

            return PayTableData.PayTable.TryGetPayout(symbolType, matchCount, out int payout) && payout > 0;
        }
    }
}
