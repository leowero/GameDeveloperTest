using Project.Slots.Domain.Engine;
using System;
using System.Collections.Generic;

namespace Project.Core.SlotMachine
{
    /// <summary>
    /// Finite state machine (FSM) responsible for controlling the main gameplay flow
    /// of the slot machine.
    /// </summary>
    /// <remarks>
    /// States must be registered before they can be used. This implementation reuses
    /// the same state instances across transitions, meaning state objects may keep
    /// internal data between activations.
    /// </remarks>
    public class GameStateMachine
    {
        private IGameState _CurrentState;
        private readonly Dictionary<Type, IGameState> _States = new Dictionary<Type, IGameState>();
        private bool _IsTransitioning;

        /// <summary>
        /// Gets the currently active state.
        /// </summary>
        /// <remarks>
        /// Returns <c>null</c> if no state has been entered yet.
        /// </remarks>
        public IGameState CurrentState => _CurrentState;

        /// <summary>
        /// Registers a state instance for a given state type.
        /// </summary>
        /// <typeparam name="T">
        /// Concrete state type that implements <see cref="IGameState"/>.
        /// </typeparam>
        /// <param name="state">
        /// Instance of the state to associate with the given type.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="state"/> is <c>null</c>.
        /// </exception>
        /// <remarks>
        /// If the same state type is registered multiple times, the last provided instance
        /// overwrites the previous one.
        /// </remarks>
        public void RegisterState<T>(T state) where T : IGameState
        {
            if (state == null)
            {
                throw new ArgumentNullException(nameof(state));
            }

            _States[typeof(T)] = state;
        }

        /// <summary>
        /// Changes the current state to the registered state of type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">
        /// Concrete state type that implements <see cref="IGameState"/>.
        /// </typeparam>
        /// <exception cref="InvalidOperationException">
        /// Thrown when attempting to change to a state that has not been registered.
        /// </exception>
        /// <remarks>
        /// Transitions are guarded to prevent re-entrant state changes. If a transition
        /// is already in progress, subsequent calls to this method are ignored.
        /// </remarks>
        public void ChangeState<T>() where T : IGameState
        {
            if (_IsTransitioning)
            {
                return;
            }

            if (!_States.TryGetValue(typeof(T), out IGameState newState) || newState == null)
            {
                throw new InvalidOperationException($"State {typeof(T).Name} not registered.");
            }

            _IsTransitioning = true;

            try
            {
                _CurrentState?.Exit();
                _CurrentState = newState;
                _CurrentState.Enter();
            }
            finally
            {
                _IsTransitioning = false;
            }
        }

        /// <summary>
        /// Executes the current state's main action and returns the resulting spin outcome.
        /// </summary>
        /// <returns>
        /// A <see cref="SpinResult"/> produced by the current state, or <c>null</c> if no
        /// state is currently active.
        /// </returns>
        /// <remarks>
        /// This method acts as the main execution entry point for the state machine.
        /// The actual behavior depends on the concrete implementation of the active state.
        /// </remarks>
        public SpinResult Action()
        {
            return _CurrentState?.Action();
        }
    }
}