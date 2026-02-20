using System.Collections.Generic;

namespace Project.Slots.Domain.Symbols
{
    public static class SymbolsConstants
    {
        private const char BELL = 'B';
        private const char CHERRY = 'C';
        private const char GRAPES = 'G';
        private const char LEMON = 'L';
        private const char ORANGE = 'O';
        private const char PLUM = 'P';
        private const char WATERMELON = 'W';

        public static readonly SymbolType BellSymbol = new SymbolType(BELL);
        public static readonly SymbolType CherrySymbol = new SymbolType(CHERRY);
        public static readonly SymbolType GrapesSymbol = new SymbolType(GRAPES);
        public static readonly SymbolType LemonSymbol = new SymbolType(LEMON);
        public static readonly SymbolType OrangeSymbol = new SymbolType(ORANGE);
        public static readonly SymbolType PlumSymbol = new SymbolType(PLUM);
        public static readonly SymbolType WatermelonSymbol = new SymbolType(WATERMELON);

        public static Dictionary<char, SymbolType> SymbolsMapping = new Dictionary<char, SymbolType>
        {
            { BELL, BellSymbol },
            { CHERRY, CherrySymbol },
            { GRAPES, GrapesSymbol },
            { LEMON, LemonSymbol },
            { ORANGE, OrangeSymbol },
            { PLUM, PlumSymbol },
            { WATERMELON, WatermelonSymbol }
        };
    }
}
