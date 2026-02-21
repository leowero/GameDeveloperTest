namespace Project.Slots.Domain.Configuration.Definitions
{
    /// <summary>
    /// Static configuration defining core slot machine layout parameters.
    /// </summary>
    /// <remarks>
    /// This class defines the structural dimensions of the slot machine grid.
    /// These values are treated as global constants and are assumed to be
    /// consistent with reel strips, pattern definitions, and pay table
    /// configuration.
    /// </remarks>
    public static class SlotDefinition
    {
        /// <summary>
        /// Number of reel columns in the slot machine grid.
        /// </summary>
        /// <remarks>
        /// This value must match the number of reel strips defined in the configuration
        /// and the number of column segments defined in each <see cref="Data.Pattern"/>.
        /// </remarks>
        public const int Columns = 5;

        /// <summary>
        /// Number of visible rows in the slot machine grid.
        /// </summary>
        /// <remarks>
        /// This value defines the height of the grid and must match the length of each
        /// column descriptor in pattern definitions.
        /// </remarks>
        public const int Rows = 3;
    }
}