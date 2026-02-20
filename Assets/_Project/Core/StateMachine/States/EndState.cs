using Project.Slots.Domain.Engine;

namespace Project.Core.SlotMachine.States
{
    public class EndState : IGameState
    {
        public void Enter()
        {
            return;
        }

        public void Exit()
        {
            return;
        }

        public SpinResult Action()
        {
            return null;
        }
    }
}