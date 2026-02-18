using UnityEngine;

namespace Project.Core.SlotMachine
{
    public interface IGameState
    {
        public void Enter();
        public void Action();
        public void Exit();
    }
}