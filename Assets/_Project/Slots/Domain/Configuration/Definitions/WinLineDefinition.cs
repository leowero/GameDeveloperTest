namespace Project.Slots.Domain.Configuration.Definitions
{
    public class WinLineDefinition
    {
        public readonly int PatternId;
        public readonly int MatchCount;
        public readonly char SymbolId;
        public readonly double Payout;

        public WinLineDefinition(int patternId, int matchCount, char symbolId, double payout)
        {
            PatternId = patternId;
            MatchCount = matchCount;
            SymbolId = symbolId;
            Payout = payout;
        }
    }
}