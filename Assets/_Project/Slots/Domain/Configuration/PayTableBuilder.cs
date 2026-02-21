using Project.Slots.Domain.Configuration.Definitions;
using System;
using System.Collections.Generic;

namespace Project.Slots.Domain.Configuration
{
    /// <summary>
    /// Builder used to compose a <see cref="PayTable"/> from per-symbol payout definitions.
    /// </summary>
    /// <remarks>
    /// This builder enforces uniqueness of symbol ids at build time.
    /// </remarks>
    public sealed class PayTableBuilder
    {
        private readonly Dictionary<char, SymbolPayDefinition> _Definitions = new Dictionary<char, SymbolPayDefinition>();

        /// <summary>
        /// Adds a symbol payout definition built by the provided builder.
        /// </summary>
        /// <param name="builder">Builder that produces a <see cref="SymbolPayDefinition"/>.</param>
        /// <returns>The same builder instance for fluent chaining.</returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="builder"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// Thrown when the symbol id was already added.
        /// </exception>
        public PayTableBuilder AddSymbol(SymbolPayBuilder builder)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            SymbolPayDefinition definition = builder.Build();

            if (_Definitions.ContainsKey(definition.SymbolId))
            {
                throw new InvalidOperationException($"Symbol '{definition.SymbolId}' already added.");
            }

            _Definitions[definition.SymbolId] = definition;
            return this;
        }

        /// <summary>
        /// Builds a <see cref="PayTable"/> instance using the accumulated definitions.
        /// </summary>
        /// <returns>A new <see cref="PayTable"/>.</returns>
        public PayTable Build()
        {
            return new PayTable(_Definitions);
        }
    }
}