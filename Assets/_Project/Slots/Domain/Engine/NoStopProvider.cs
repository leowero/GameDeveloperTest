namespace Project.Slots.Domain.Engine
{
    /// <summary>
    /// Default stop provider that never forces stop indexes.
    /// </summary>
    /// <remarks>
    ///
    /// This implementation exists as a null object to avoid passing <c>null</c> providers.
    /// </remarks>
    public sealed class NoStopProvider : ISpinStopProvider
    {
        /// <summary>
        /// Always returns <c>false</c>, indicating that no forced stop indexes are available.
        /// </summary>
        /// <param name="stopIndexes">Always <c>null</c>.</param>
        /// <returns>Always <c>false</c>.</returns>
        public bool TryGetStopIndexes(out int[] stopIndexes)
        {
            stopIndexes = null;
            return false;
        }
    }
}