namespace Project.Slots.Domain.Cheats
{
    public sealed class CheatRequest
    {
        public CheatKind Kind { get; }
        public char SymbolId { get; }
        public int MatchCount { get; }
        public int? PatternId { get; }

        private CheatRequest(CheatKind kind, char symbolId, int matchCount, int? patternId)
        {
            Kind = kind;
            SymbolId = symbolId;
            MatchCount = matchCount;
            PatternId = patternId;
        }

        public static CheatRequest AnyPattern(char symbolId, int matchCount) => new(CheatKind.ForceWinAnyPattern, symbolId, matchCount, null);

        public static CheatRequest SpecificPattern(char symbolId, int matchCount, int patternId) => new(CheatKind.ForceWinSpecificPattern, symbolId, matchCount, patternId);
    }
}
