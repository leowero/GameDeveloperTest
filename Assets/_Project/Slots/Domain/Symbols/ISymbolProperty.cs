namespace Project.Slots.Domain.Symbols
{
    /// <summary>
    /// Defines a property that can be attached to a symbol to extend its behavior or metadata.
    /// </summary>
    /// <remarks>
    /// Symbol properties are intended to model special behaviors or attributes associated
    /// with a symbol, such as wild behavior, scatter rules, multipliers, bonus triggers,
    /// or other game-specific mechanics.
    ///
    /// Implementations of this interface are typically used by higher-level gameplay logic
    /// when evaluating spins, payouts, or triggering special features.
    /// </remarks>
    public interface ISymbolProperty
    {
        /// <summary>
        /// Human-readable name of the symbol property.
        /// </summary>
        /// <remarks>
        /// This name is intended for identification, debugging, or tooling purposes.
        /// It is not required to be unique unless enforced by higher-level systems.
        /// </remarks>
        string Name { get; set; }
    }
}