using Project.Core.SlotMachine;
using Project.Core.SlotMachine.States;
using Project.Slots.Domain.Cheats;
using Project.Slots.Domain.Engine;
using Project.Slots.Presentation.Configuration;
using System;
using UnityEngine;

namespace Project.Slots.Presentation.Controllers
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private SlotsConfiguration _Configuration;
        public event Action OnSpinStarted;
        public event Action<SpinResult> OnSpinResolved;
        public event Action OnReelsStopped;

        private readonly GameStateMachine _StateMachine = new GameStateMachine();
        private CheatStopProvider _CheatProvider;

        public static GameManager Instance { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }

        private void Start()
        {
            _CheatProvider = new CheatStopProvider(_Configuration.Patterns);

            _StateMachine.RegisterState(new StartState());
            _StateMachine.RegisterState(new SpinState(_Configuration.Patterns, _CheatProvider));
            _StateMachine.RegisterState(new EndState());

            _StateMachine.ChangeState<StartState>();
        }

        public CheatStopProvider GetCheatProvider() => _CheatProvider;

        public void Spin()
        {
            OnSpinStarted?.Invoke();

            _StateMachine.ChangeState<SpinState>();
            SpinResult result = _StateMachine.Action();
            OnSpinResolved?.Invoke(result);
        }

        public void NotifyReelsStopped()
        {
            OnReelsStopped?.Invoke();
        }
    }
}