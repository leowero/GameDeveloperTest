using Project.Slots.Domain.Configuration.Definitions;
using Project.Slots.Domain.Symbols;
using System.Collections.Generic;

namespace Project.Slots.Domain.Configuration
{
    public class PayTableBuilder
    {
        private readonly Dictionary<SymbolType, SymbolPayDefinition> _Definitions = new Dictionary<SymbolType, SymbolPayDefinition>();

        public PayTableBuilder AddSymbol(SymbolPayBuilder builder)
        {
            var definition = builder.Build();

            if (_Definitions.ContainsKey(definition.SymbolType))
            {
                throw new System.Exception($"Symbol {definition.SymbolType} already added.");
            }
            _Definitions[definition.SymbolType] = definition;

            return this;
        }

        public PayTable Build()
        {
            return new PayTable(_Definitions);
        }
    }
}