namespace Project.Slots.Domain.Cheats
{
    /// <summary>
    /// Defines the supported cheat request types.
    /// </summary>
    /// <remarks>
    /// Cheats are used to force deterministic outcomes for debugging, testing,
    /// and development workflows.
    /// </remarks>
    public enum CheatKind
    {
        /// <summary>
        /// Force a win on the first pattern that can satisfy the request.
        /// </summary>
        ForceWinAnyPattern,

        /// <summary>
        /// Force a win on a specific pattern id only.
        /// </summary>
        ForceWinSpecificPattern
    }
}