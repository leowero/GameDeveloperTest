using Project.Slots.Domain.Configuration.Definitions;
using System.Collections.Generic;

namespace Project.Slots.Domain.Reels
{
    /// <summary>
    /// Defines the reel strips used by the slot machine engine.
    /// </summary>
    /// <remarks>
    /// Each reel strip is represented as a circular sequence of symbol identifiers.
    /// The engine selects a contiguous window of symbols from each strip to populate
    /// the visible grid for a spin.
    ///
    /// This class validates its configuration at initialization time to ensure
    /// consistency with <see cref="SlotDefinition"/>. Invalid configuration results
    /// in an exception during application startup rather than silent runtime failures.
    /// </remarks>
    public static class ReelStrips
    {
        private static readonly string[] _Reels = new string[]
        {
            "OBWCPLGPBBOGLL",
            "WCBPCGOLLLCLPLC",
            "GWPGBLCBLLOOG",
            "LPPLGOWWBCCLOPL",
            "GCBWOOLPOLGBWC"
        };

        /// <summary>
        /// Read-only list of reel strips used by the slot machine.
        /// </summary>
        /// <remarks>
        /// The number of reels must match <see cref="SlotDefinition.Columns"/>.
        /// Each reel must have a length greater than or equal to
        /// <see cref="SlotDefinition.Rows"/> to support extracting the visible grid window.
        /// </remarks>
        public static IReadOnlyList<string> Reels => _Reels;
    }
}