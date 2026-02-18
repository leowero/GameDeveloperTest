using Project.Slots.Domain.Configuration.Definitions;
using Project.Slots.Domain.Engine;
using Project.Slots.Domain.Symbols;
using Project.Slots.Presentation.Controllers;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Project.Core.SlotMachine.States
{
    public class SpinState : IGameState
    {
        public void Enter()
        {
        }

        public void Exit()
        {
            
        }

        public void Action()
        {
            GameManager.Instance.Grid = SlotMachineEngine.Spin();

            List<WinLineDefinition> wins = SlotMachineEngine.CompileWins(GameManager.Instance.Grid, GameManager.Instance.Patterns);

            foreach (var win in wins)
            {
                Debug.Log($"Win id: {win.PatternId}, Match Count: {win.MatchCount}, Symbol: {win.SymbolId}, Payout: {win.Payout}");
            }
            if (wins.Sum(w => w.Payout) > 0)
            {
                Debug.Log($"Total payout: {wins.Sum(w => w.Payout)}");
            }
        }
    }
}
