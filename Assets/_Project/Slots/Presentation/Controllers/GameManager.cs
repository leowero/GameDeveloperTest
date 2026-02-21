using Project.Core.SlotMachine;
using Project.Core.SlotMachine.States;
using Project.Slots.Domain.Cheats;
using Project.Slots.Domain.Engine;
using Project.Slots.Presentation.Configuration;
using System;
using UnityEngine;

namespace Project.Slots.Presentation.Controllers
{
    /// <summary>
    /// Coordinates slot machine gameplay flow at the presentation layer.
    /// </summary>
    /// <remarks>
    /// Responsibilities:
    /// - Owns the <see cref="GameStateMachine"/> used to drive the spin lifecycle.
    /// - Wires configured patterns and cheat providers into gameplay states.
    /// - Exposes events used by UI and presentation systems to react to spin lifecycle milestones.
    ///
    /// This class follows a simple singleton pattern to allow UI components to locate it easily.
    /// </remarks>
    public sealed class GameManager : MonoBehaviour
    {
        /// <summary>
        /// Slot configuration used to initialize the state machine and gameplay dependencies.
        /// </summary>
        [SerializeField] private SlotsConfiguration _Configuration;

        /// <summary>
        /// Raised when a spin is requested and the spin flow starts.
        /// </summary>
        public event Action OnSpinStarted;

        /// <summary>
        /// Raised when the engine has produced a <see cref="SpinResult"/> for the current spin.
        /// </summary>
        public event Action<SpinResult> OnSpinResolved;

        /// <summary>
        /// Raised when the reel presentation signals that all reels have visually stopped.
        /// </summary>
        public event Action OnReelsStopped;

        private readonly GameStateMachine _StateMachine = new GameStateMachine();
        private CheatStopProvider _CheatProvider;

        /// <summary>
        /// Singleton instance of the <see cref="GameManager"/>.
        /// </summary>
        /// <remarks>
        /// This is set during <see cref="Awake"/> and cleared on <see cref="OnDestroy"/>.
        /// </remarks>
        public static GameManager Instance { get; private set; }

        /// <summary>
        /// Cheat stop provider used by the spin state to optionally force deterministic stops.
        /// </summary>
        /// <remarks>
        /// Exposed as read-only to avoid reassignments. Consumers can enqueue cheat requests
        /// through this provider when debugging.
        /// </remarks>
        public CheatStopProvider CheatProvider => _CheatProvider;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }

        private void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
            }
        }

        private void Start()
        {
            if (_Configuration == null)
            {
                Debug.LogError($"{nameof(GameManager)} requires a {nameof(SlotsConfiguration)} reference.", this);
                enabled = false;
                return;
            }

            _CheatProvider = new CheatStopProvider(_Configuration.Patterns);

            _StateMachine.RegisterState(new StartState());
            _StateMachine.RegisterState(new SpinState(_Configuration.Patterns, _CheatProvider));
            _StateMachine.RegisterState(new EndState());

            _StateMachine.ChangeState<StartState>();
        }

        /// <summary>
        /// Starts a spin by transitioning into the spin state and executing the state's action.
        /// </summary>
        /// <remarks>
        /// This method triggers:
        /// - <see cref="OnSpinStarted"/>,
        /// - A state transition to <see cref="SpinState"/>,
        /// - The spin action producing a <see cref="SpinResult"/>,
        /// - <see cref="OnSpinResolved"/> with the produced result.
        ///
        /// Visual reel spinning and reel stop timing are expected to be handled by presentation systems.
        /// Those systems should call <see cref="NotifyReelsStopped"/> when the animation completes.
        /// </remarks>
        public void Spin()
        {
            OnSpinStarted?.Invoke();

            _StateMachine.ChangeState<SpinState>();
            SpinResult result = _StateMachine.Action();
            OnSpinResolved?.Invoke(result);
        }

        /// <summary>
        /// Notifies listeners that reels have visually stopped.
        /// </summary>
        /// <remarks>
        /// This is intended to be called by the reel animation/presentation layer when
        /// all reel stop animations have completed.
        /// </remarks>
        public void NotifyReelsStopped()
        {
            OnReelsStopped?.Invoke();
        }
    }
}