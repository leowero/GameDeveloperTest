using Project.Slots.Domain.Engine;
using System;
using System.Collections.Generic;

namespace Project.Core.SlotMachine
{
    public class GameStateMachine
    {
        private IGameState _CurrentState;
        private readonly Dictionary<Type, IGameState> _States = new Dictionary<Type, IGameState>();
        private bool _IsTransitioning;

        public IGameState CurrentState => _CurrentState;

        public void RegisterState<T>(T state) where T : IGameState
        {
            _States[typeof(T)] = state;
        }

        public void ChangeState<T>() where T : IGameState
        {
            if (_IsTransitioning)
            {
                return;
            }

            if (!_States.TryGetValue(typeof(T), out IGameState newState))
            {
                throw new Exception($"State {typeof(T)} not registered.");
            }

            _IsTransitioning = true;

            _CurrentState?.Exit();
            _CurrentState = newState;
            _CurrentState.Enter();

            _IsTransitioning = false;
        }

        public SpinResult Action()
        {
            return _CurrentState?.Action();
        }
    }
}
