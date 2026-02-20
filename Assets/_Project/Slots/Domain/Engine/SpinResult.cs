using Project.Slots.Domain.Configuration.Definitions;
using System.Collections.Generic;
using System.Linq;

namespace Project.Slots.Domain.Engine
{
    public class SpinResult
    {
        public readonly IReadOnlyList<WinLineDefinition> Wins;
        public readonly int[] StopIndexes;
        public readonly double TotalPayout;

        public SpinResult(IReadOnlyList<WinLineDefinition> wins, int[] stopIndexes)
        {
            Wins = wins;
            StopIndexes = stopIndexes;
            TotalPayout = wins.Sum(w => w.Payout);
        }
    }
}