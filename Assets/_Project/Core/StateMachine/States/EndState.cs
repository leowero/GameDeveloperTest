using Project.Slots.Domain.Engine;

namespace Project.Core.SlotMachine.States
{
    /// <summary>
    /// Terminal state of the slot machine flow.
    /// </summary>
    /// <remarks>
    /// This state represents the end of a gameplay cycle or session.
    /// It can be used as a stable resting point after payouts, session termination,
    /// or before resetting the state machine back to the start or idle state.
    /// In its current form, this implementation acts as a no-op placeholder.
    /// </remarks>
    public class EndState : IGameState
    {
        /// <summary>
        /// Called when the state becomes active.
        /// </summary>
        /// <remarks>
        /// Hook for end-of-session logic. The current implementation
        /// performs no operation.
        /// </remarks>
        public void Enter()
        {
            return;
        }

        /// <summary>
        /// Executes the main logic of the end state.
        /// </summary>
        /// <returns>
        /// Always returns <c>null</c> in the current implementation, as this state does not
        /// produce gameplay results.
        /// </returns>
        /// <remarks>
        /// This state is not intended to perform gameplay actions. It exists as a terminal
        /// or transitional phase in the overall state machine lifecycle.
        /// </remarks>
        public SpinResult Action()
        {
            return null;
        }

        /// <summary>
        /// Called when the state is exited.
        /// </summary>
        /// <remarks>
        /// Hook for cleanup logic related to the end-of-session flow.
        /// The current implementation performs no operation.
        /// </remarks>
        public void Exit()
        {
            return;
        }
    }
}