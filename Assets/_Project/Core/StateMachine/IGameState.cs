using Project.Slots.Domain.Engine;

namespace Project.Core.SlotMachine
{
    /// <summary>
    /// Defines the contract for a single state within the slot machine state machine.
    /// </summary>
    /// <remarks>
    /// Each state represents a distinct phase of the slot machine flow.
    /// Implementations are expected to be reused across multiple activations,
    /// so they may keep internal state between calls to <see cref="Enter"/> and <see cref="Exit"/>.
    /// </remarks>
    public interface IGameState
    {
        /// <summary>
        /// Called when the state becomes the active state.
        /// </summary>
        /// <remarks>
        /// This method is typically used to initialize or reset any transient data
        /// required for the state's execution.
        /// </remarks>
        void Enter();

        /// <summary>
        /// Executes the main logic associated with this state.
        /// </summary>
        /// <returns>
        /// A <see cref="SpinResult"/> representing the outcome of the state's action.
        /// The meaning of this result depends on the concrete state implementation.
        /// </returns>
        /// <remarks>
        /// This method is expected to be invoked by the state machine as part of the
        /// main gameplay loop. Implementations should avoid performing state transitions
        /// directly and instead communicate intent via returned results or events.
        /// </remarks>
        SpinResult Action();

        /// <summary>
        /// Called when the state is no longer the active state.
        /// </summary>
        /// <remarks>
        /// This method is typically used to clean up temporary resources or
        /// finalize any pending operations before leaving the state.
        /// </remarks>
        void Exit();
    }
}