namespace Project.Slots.Domain.Symbols
{
    public class SymbolType
    {
        public char Type;
        public ISymbolProperty[] Properties;

        public SymbolType(char type, ISymbolProperty[] properties = null)
        {
            Type = type;
            Properties = properties;
        }
    }
}