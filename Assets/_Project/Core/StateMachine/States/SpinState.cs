using Project.Slots.Data;
using Project.Slots.Domain.Configuration.Definitions;
using Project.Slots.Domain.Engine;
using Project.Slots.Domain.Symbols;
using System.Collections.Generic;

namespace Project.Core.SlotMachine.States
{
    public class SpinState : IGameState
    {
        private readonly IReadOnlyList<Pattern> _Patterns;
        private ISpinStopProvider _StopProvider;

        public SpinState(IReadOnlyList<Pattern> patterns, ISpinStopProvider stopProvider)
        {
            _Patterns = patterns;
            _StopProvider = stopProvider;
        }

        public void Enter()
        {
        }

        public void Exit()
        {
            
        }

        public SpinResult Action()
        {
            int[] forcedStopIndexes = null;
            if (_StopProvider != null && _StopProvider.TryGetStopIndexes(out int[] forced))
            {
                forcedStopIndexes = forced;
            }

            SymbolType[][] grid = SlotMachineEngine.Spin(out int[] stopIndexes, forcedStopIndexes);
            IReadOnlyList<WinLineDefinition> wins = SlotMachineEngine.CompileWins(grid, _Patterns);

            return new SpinResult(wins, stopIndexes);
        }
    }
}
