namespace Project.Slots.Domain.Symbols
{
    public class WildProperty : ISymbolProperty
    {
        public string Name { get; set; }

        public WildProperty()
        {
            Name = "Wild";
        }
    }
}