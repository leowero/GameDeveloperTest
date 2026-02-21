using System.Collections.Generic;
using System.Linq;

namespace Project.Slots.Domain.Symbols
{
    /// <summary>
    /// Represents a symbol type used by the slot machine engine.
    /// </summary>
    /// <remarks>
    /// A symbol type encapsulates the raw identifier used in reel strips and an optional
    /// set of properties that define additional behavior or characteristics for the symbol.
    ///
    /// This type is designed to be immutable after construction. Instances are typically
    /// shared across the engine as canonical definitions of each symbol.
    /// </remarks>
    public sealed class SymbolType
    {
        /// <summary>
        /// Raw character identifier used to represent this symbol in reel strips.
        /// </summary>
        public char Type { get; }

        /// <summary>
        /// Read-only collection of properties associated with this symbol.
        /// </summary>
        public IReadOnlyList<ISymbolProperty> Properties { get; }

        /// <summary>
        /// Creates a new <see cref="SymbolType"/> with the given identifier and optional properties.
        /// </summary>
        /// <param name="type">Raw character identifier used in reel strips.</param>
        /// <param name="properties">Optional set of properties associated with this symbol.</param>
        /// <remarks>
        /// The provided properties are copied defensively to prevent external mutation after construction.
        /// </remarks>
        public SymbolType(char type, IEnumerable<ISymbolProperty> properties = null)
        {
            Type = type;
            Properties = properties != null ? properties.ToList() : new List<ISymbolProperty>();
        }
    }
}