using Project.Slots.Domain.Engine;

namespace Project.Core.SlotMachine.States
{
    /// <summary>
    /// Initial state of the slot machine flow.
    /// </summary>
    /// <remarks>
    /// This state represents the entry point of the state machine and is typically
    /// responsible for performing any initial setup required before the main
    /// gameplay loop begins. In its current form, this implementation acts as a
    /// no-op placeholder and can be extended to trigger initialization logic.
    /// </remarks>
    public class StartState : IGameState
    {
        /// <summary>
        /// Called when the state becomes active.
        /// </summary>
        /// <remarks>
        /// This method can be used to initialize game-wide systems or prepare
        /// the slot machine for the first interaction. The current implementation
        /// performs no operation.
        /// </remarks>
        public void Enter()
        {
            return;
        }

        /// <summary>
        /// Executes the main logic of the start state.
        /// </summary>
        /// <returns>
        /// Always returns <c>null</c> in the current implementation, indicating that
        /// this state does not produce a spin result.
        /// </returns>
        /// <remarks>
        /// This state does not represent an actionable gameplay step. It is intended
        /// to be a transitional or bootstrap state before moving into an interactive
        /// state such as idle or ready-to-spin.
        /// </remarks>
        public SpinResult Action()
        {
            return null;
        }

        /// <summary>
        /// Called when the state is exited.
        /// </summary>
        /// <remarks>
        /// This method can be used to clean up any temporary initialization logic.
        /// The current implementation performs no operation.
        /// </remarks>
        public void Exit()
        {
            return;
        }
    }
}