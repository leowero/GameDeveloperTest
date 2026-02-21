namespace Project.Slots.Domain.Engine
{
    /// <summary>
    /// Provides optional forced stop indexes for a slot machine spin.
    /// </summary>
    /// <remarks>
    /// Implementations can be used to produce deterministic outcomes for testing, cheats
    /// or debugging. When no forced stops are available, implementations should
    /// return <c>false</c>.
    ///
    /// Contract:
    /// If this method returns <c>true</c>, the returned array is expected to contain one stop index
    /// per reel column, in column order.
    /// </remarks>
    public interface ISpinStopProvider
    {
        /// <summary>
        /// Attempts to provide forced stop indexes for the next spin.
        /// </summary>
        /// <param name="stopIndexes">
        /// Output stop indexes when available. When the method returns <c>false</c>, this value should be <c>null</c>.
        /// </param>
        /// <returns>
        /// <c>true</c> if forced stop indexes are available, otherwise <c>false</c>.
        /// </returns>
        /// <remarks>
        /// This method does not validate compatibility with the current reel configuration.
        /// Validation is expected to be performed by the engine or higher-level systems.
        /// </remarks>
        bool TryGetStopIndexes(out int[] stopIndexes);
    }
}