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

        public SpinState(IReadOnlyList<Pattern> patterns)
        {
            _Patterns = patterns;
        }

        public void Enter()
        {
        }

        public void Exit()
        {
            
        }

        public SpinResult Action()
        {
            SymbolType[][] grid = SlotMachineEngine.Spin(out int[] stopIndexes);
            IReadOnlyList<WinLineDefinition> wins = SlotMachineEngine.CompileWins(grid, _Patterns);

            return new SpinResult(wins, stopIndexes);
        }
    }
}
