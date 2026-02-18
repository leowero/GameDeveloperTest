using System;
using System.Collections.Generic;

namespace Project.Core.SlotMachine
{
    public class GameStateMachine
    {
        public IGameState CurrentState;
        private readonly Dictionary<Type, IGameState> States = new Dictionary<Type, IGameState>();
        private bool IsTransitioning;

        public void RegisterState<T>(T state) where T : IGameState
        {
            States[typeof(T)] = state;
        }

        public void ChangeState<T>() where T : IGameState
        {
            if (IsTransitioning)
            {
                return;
            }

            if (!States.TryGetValue(typeof(T), out IGameState newState))
            {
                throw new Exception($"State {typeof(T)} not registered.");
            }

            IsTransitioning = true;

            CurrentState?.Exit();
            CurrentState = newState;
            CurrentState.Enter();

            IsTransitioning = false;
        }

        public void Action()
        {
            CurrentState?.Action();
        }
    }
}
