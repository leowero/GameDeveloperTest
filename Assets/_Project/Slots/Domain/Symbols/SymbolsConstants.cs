using System;
using System.Collections.Generic;

namespace Project.Slots.Domain.Symbols
{
    /// <summary>
    /// Centralized symbol definitions and mapping used by the slot machine engine.
    /// </summary>
    /// <remarks>
    /// This class defines the canonical set of symbol identifiers used in reel strips
    /// and provides a mapping from raw reel characters to domain-level <see cref="SymbolType"/> instances.
    ///
    /// All reel strips are expected to use only the characters defined here. Any character
    /// not present in the mapping represents an invalid configuration.
    ///
    /// The mapping is exposed as read-only to prevent accidental mutation at runtime.
    /// Configuration is validated during static initialization to fail fast on invalid setup.
    /// </remarks>
    public static class SymbolsConstants
    {
        /// <summary>
        /// Raw character identifiers used in reel strips.
        /// </summary>
        /// <remarks>
        /// These constants define the allowed symbol IDs that can appear in reel strip definitions.
        /// </remarks>
        private const char BELL = 'B';
        private const char CHERRY = 'C';
        private const char GRAPES = 'G';
        private const char LEMON = 'L';
        private const char ORANGE = 'O';
        private const char PLUM = 'P';
        private const char WATERMELON = 'W';

        /// <summary>
        /// Domain-level symbol instances corresponding to each raw symbol identifier.
        /// </summary>
        /// <remarks>
        /// These instances are treated as canonical singletons for each symbol type and are reused
        /// across all spins and grid evaluations.
        /// </remarks>
        public static readonly SymbolType BellSymbol = new SymbolType(BELL);
        public static readonly SymbolType CherrySymbol = new SymbolType(CHERRY);
        public static readonly SymbolType GrapesSymbol = new SymbolType(GRAPES);
        public static readonly SymbolType LemonSymbol = new SymbolType(LEMON);
        public static readonly SymbolType OrangeSymbol = new SymbolType(ORANGE);
        public static readonly SymbolType PlumSymbol = new SymbolType(PLUM);
        public static readonly SymbolType WatermelonSymbol = new SymbolType(WATERMELON);

        private static readonly Dictionary<char, SymbolType> _SymbolsMapping = new Dictionary<char, SymbolType>
        {
            { BELL, BellSymbol },
            { CHERRY, CherrySymbol },
            { GRAPES, GrapesSymbol },
            { LEMON, LemonSymbol },
            { ORANGE, OrangeSymbol },
            { PLUM, PlumSymbol },
            { WATERMELON, WatermelonSymbol }
        };

        /// <summary>
        /// Read-only mapping from raw reel characters to domain-level symbol definitions.
        /// </summary>
        /// <remarks>
        /// This mapping is used by the slot engine to translate reel strip characters
        /// into concrete <see cref="SymbolType"/> instances when building the visible grid.
        ///
        /// The mapping is expected to be complete with respect to all characters that appear
        /// in reel strips. Missing keys indicate invalid configuration and will result in runtime errors
        /// when resolving symbols during a spin.
        /// </remarks>
        public static IReadOnlyDictionary<char, SymbolType> SymbolsMapping => _SymbolsMapping;
    }
}