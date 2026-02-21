namespace Project.Slots.Domain.Configuration.Definitions
{
    /// <summary>
    /// Represents a single winning line evaluation result for a spin.
    /// </summary>
    /// <remarks>
    /// A win line definition captures the evaluated outcome of applying a specific pattern
    /// to a spin result, including how many consecutive symbols matched and the resulting payout.
    /// This type is designed as a value object and is not intended to be modified after creation.
    /// </remarks>
    public sealed class WinLineDefinition
    {
        /// <summary>
        /// Identifier of the pattern that produced this win.
        /// </summary>
        public int PatternId { get; }

        /// <summary>
        /// Number of consecutive matching symbols from the first column.
        /// </summary>
        public int MatchCount { get; }

        /// <summary>
        /// Identifier of the symbol type that produced the match.
        /// </summary>
        public char SymbolId { get; }

        /// <summary>
        /// Payout awarded for this win line.
        /// </summary>
        public double Payout { get; }

        /// <summary>
        /// Creates a new <see cref="WinLineDefinition"/>.
        /// </summary>
        /// <param name="patternId">Identifier of the evaluated pattern.</param>
        /// <param name="matchCount">Number of consecutive matching symbols.</param>
        /// <param name="symbolId">Identifier of the matched symbol.</param>
        /// <param name="payout">Payout awarded for this win.</param>
        /// <remarks>
        /// This constructor does not enforce domain invariants. It assumes that the provided values
        /// have already been validated by the slot engine and pay table configuration. The class
        /// acts as a simple data carrier for evaluated win results.
        /// </remarks>
        public WinLineDefinition(int patternId, int matchCount, char symbolId, double payout)
        {
            PatternId = patternId;
            MatchCount = matchCount;
            SymbolId = symbolId;
            Payout = payout;
        }
    }
}