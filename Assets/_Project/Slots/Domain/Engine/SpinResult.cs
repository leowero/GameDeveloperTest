using Project.Slots.Domain.Configuration.Definitions;
using System.Collections.Generic;
using System.Linq;

namespace Project.Slots.Domain.Engine
{
    public class SpinResult
    {
        public readonly List<WinLineDefinition> Wins;
        public readonly double TotalPayout;

        public SpinResult(List<WinLineDefinition> wins)
        {
            Wins = wins;
            TotalPayout = wins.Sum(w => w.Payout);
        }
    }
}