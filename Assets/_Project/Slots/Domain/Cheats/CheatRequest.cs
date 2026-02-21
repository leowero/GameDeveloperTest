namespace Project.Slots.Domain.Cheats
{
    /// <summary>
    /// Represents a parsed cheat command to be consumed by cheat systems.
    /// </summary>
    /// <remarks>
    /// A cheat request defines the desired symbol id and match count, and optionally
    /// a specific pattern id depending on <see cref="Kind"/>.
    /// </remarks>
    public sealed class CheatRequest
    {
        /// <summary>
        /// The type of cheat being requested.
        /// </summary>
        public CheatKind Kind { get; }

        /// <summary>
        /// Raw symbol identifier that should appear in the forced win.
        /// </summary>
        public char SymbolId { get; }

        /// <summary>
        /// Number of consecutive matching columns requested.
        /// </summary>
        public int MatchCount { get; }

        /// <summary>
        /// Optional target pattern id when <see cref="Kind"/> is <see cref="CheatKind.ForceWinSpecificPattern"/>.
        /// </summary>
        public int? PatternId { get; }

        private CheatRequest(CheatKind kind, char symbolId, int matchCount, int? patternId)
        {
            Kind = kind;
            SymbolId = symbolId;
            MatchCount = matchCount;
            PatternId = patternId;
        }

        /// <summary>
        /// Creates a request to force a win on any satisfiable pattern.
        /// </summary>
        public static CheatRequest AnyPattern(char symbolId, int matchCount)
        {
            return new CheatRequest(CheatKind.ForceWinAnyPattern, symbolId, matchCount, null);
        }

        /// <summary>
        /// Creates a request to force a win on a specific pattern id.
        /// </summary>
        public static CheatRequest SpecificPattern(char symbolId, int matchCount, int patternId)
        {
            return new CheatRequest(CheatKind.ForceWinSpecificPattern, symbolId, matchCount, patternId);
        }
    }
}