using Project.Slots.Data;

namespace Project.Slots.Domain.Configuration
{
    /// <summary>
    /// Helper rules related to pay table validation and gameplay queries.
    /// </summary>
    /// <remarks>
    /// This class provides simple checks against the configured pay table.
    /// It does not compute spins or evaluate patterns.
    /// </remarks>
    public static class PayTableRules
    {
        /// <summary>
        /// Determines whether a match count is valid for a given symbol id based on the configured pay table.
        /// </summary>
        /// <param name="symbolId">Raw symbol identifier.</param>
        /// <param name="matchCount">Match count to validate.</param>
        /// <returns>
        /// <c>true</c> if the pay table contains a payout greater than zero for the given input, otherwise <c>false</c>.
        /// </returns>
        /// <remarks>
        /// This is a convenience method, it queries <see cref="PayTableData"/> directly.
        /// </remarks>
        public static bool IsValidMatchCount(char symbolId, int matchCount)
        {
            return PayTableData.PayTable.TryGetPayout(symbolId, matchCount, out int payout) && payout > 0;
        }
    }
}