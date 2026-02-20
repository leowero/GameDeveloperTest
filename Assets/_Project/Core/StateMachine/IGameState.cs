using Project.Slots.Domain.Engine;

namespace Project.Core.SlotMachine
{
    public interface IGameState
    {
        public void Enter();
        public SpinResult Action();
        public void Exit();
    }
}