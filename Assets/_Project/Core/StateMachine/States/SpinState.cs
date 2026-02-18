using Project.Slots.Domain.Engine;
using Project.Slots.Domain.Symbols;
using Project.Slots.Presentation.Controllers;
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
        }
    }
}
