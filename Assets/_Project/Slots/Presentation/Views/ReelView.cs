using Project.Slots.Domain.Configuration.Definitions;
using Project.Slots.Domain.Reels;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Project.Slots.Presentation.Views
{
    /// <summary>
    /// Visual representation of a single reel column.
    /// </summary>
    /// <remarks>
    /// This component is responsible for:
    /// - Rendering the current visible symbols for the reel based on a reel strip,
    /// - Animating the reel while spinning,
    /// - Decelerating and stopping on a target stop index.
    ///
    /// The reel strip is treated as circular. The visible window size is defined by
    /// <see cref="SlotDefinition.Rows"/>.
    /// </remarks>
    public sealed class ReelView : MonoBehaviour
    {
        [SerializeField] private Image[] _Symbols;
        [SerializeField] private VisualSymbol[] _VisualSymbols;
        [SerializeField] private int _ReelIndex;

        [Header("Stop behavior")]
        [SerializeField] private int _StopDurationMs = 350;
        [SerializeField] private int _MinStopSteps = 6;

        private Dictionary<char, Sprite> _SymbolMap;
        private TaskCompletionSource<bool> _StopCompletion;

        private string _Strip;
        private int _CurrentIndex;

        private bool _Disposed;

        private CancellationTokenSource _SpinLoopCts;
        private Task _SpinLoopTask;

        private void Awake()
        {
            InitializeStrip();
            InitializeSymbolMap();
            RenderCurrent();
        }

        /// <summary>
        /// Starts the reel spin loop.
        /// </summary>
        /// <remarks>
        /// If already spinning, this method returns immediately.
        /// The loop advances the reel at a fixed tick rate until <see cref="StopSpin(int)"/> is called
        /// or the object is destroyed/disabled.
        /// </remarks>
        public Task StartSpin()
        {
            if (_Disposed)
            {
                return Task.CompletedTask;
            }

            if (_SpinLoopTask != null && !_SpinLoopTask.IsCompleted)
            {
                return _SpinLoopTask;
            }

            if (string.IsNullOrEmpty(_Strip))
            {
                return Task.CompletedTask;
            }

            _SpinLoopCts = new CancellationTokenSource();
            _SpinLoopTask = SpinLoop(_SpinLoopCts.Token);
            return _SpinLoopTask;
        }

        /// <summary>
        /// Stops the reel on the provided stop index.
        /// </summary>
        /// <param name="stopIndex">Target stop index on the reel strip.</param>
        /// <remarks>
        /// This method cancels the free-spin loop and performs a controlled stepping sequence to land
        /// on the requested stop index. The intended duration of the stopping sequence is approximately
        /// <see cref="_StopDurationMs"/>, subject to frame/timer scheduling.
        /// </remarks>
        public async Task StopSpin(int stopIndex)
        {
            if (_Disposed)
            {
                return;
            }

            if (string.IsNullOrEmpty(_Strip))
            {
                return;
            }

            CancelSpinLoop();

            _StopCompletion = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);

            int stripLength = _Strip.Length;
            int normalizedStopIndex = Mod(stopIndex, stripLength);

            int rawSteps = (normalizedStopIndex - _CurrentIndex) % stripLength;
            if (rawSteps < 0)
            {
                rawSteps += stripLength;
            }

            int steps = rawSteps;
            if (steps < _MinStopSteps)
            {
                steps += stripLength;
            }

            int perStepDelay = 1;
            if (steps > 0 && _StopDurationMs > 0)
            {
                perStepDelay = Mathf.Max(1, _StopDurationMs / steps);
            }

            int safety = stripLength * 3;
            while (steps-- > 0 && safety-- > 0 && !_Disposed)
            {
                Advance();
                await Task.Delay(perStepDelay);
            }

            _CurrentIndex = normalizedStopIndex;
            RenderCurrent();

            _StopCompletion.TrySetResult(true);
        }

        /// <summary>
        /// Returns a task that completes once the current stop operation finishes.
        /// </summary>
        /// <remarks>
        /// If no stop operation is in progress, the returned task is already completed.
        /// </remarks>
        public Task WaitUntilStopped()
        {
            return _StopCompletion != null ? _StopCompletion.Task : Task.CompletedTask;
        }

        /// <summary>
        /// Gets the world-space center point of the symbol image at the provided row.
        /// </summary>
        /// <param name="row">Row index in the visible reel window.</param>
        /// <returns>World-space center position for the symbol image.</returns>
        public Vector3 GetSymbolWorldCenter(int row)
        {
            if (_Symbols == null || row < 0 || row >= _Symbols.Length)
            {
                return transform.position;
            }

            if (_Symbols[row] == null)
            {
                return transform.position;
            }

            RectTransform rt = _Symbols[row].rectTransform;
            Vector3 localCenter = rt.rect.center;
            return rt.TransformPoint(localCenter);
        }

        private async Task SpinLoop(CancellationToken token)
        {
            int tickMs = 50;

            while (!token.IsCancellationRequested && !_Disposed)
            {
                Advance();
                await Task.Delay(tickMs, token);
            }
        }

        private void CancelSpinLoop()
        {
            if (_SpinLoopCts == null)
            {
                return;
            }

            if (!_SpinLoopCts.IsCancellationRequested)
            {
                _SpinLoopCts.Cancel();
            }

            _SpinLoopCts.Dispose();
            _SpinLoopCts = null;
        }

        private void InitializeStrip()
        {
            IReadOnlyList<string> reels = ReelStrips.Reels;

            if (reels == null || reels.Count == 0)
            {
                Debug.LogError($"{nameof(ReelView)} cannot initialize, ReelStrips.Reels is empty.", this);
                _Strip = null;
                return;
            }

            if (_ReelIndex < 0 || _ReelIndex >= reels.Count)
            {
                Debug.LogError($"{nameof(ReelView)} has invalid reel index {_ReelIndex}.", this);
                _Strip = null;
                return;
            }

            _Strip = reels[_ReelIndex];
        }

        private void InitializeSymbolMap()
        {
            _SymbolMap = new Dictionary<char, Sprite>();

            if (_VisualSymbols == null || _VisualSymbols.Length == 0)
            {
                Debug.LogWarning($"{nameof(ReelView)} has no visual symbols assigned.", this);
                return;
            }

            foreach (VisualSymbol vs in _VisualSymbols)
            {
                if (vs == null)
                {
                    continue;
                }

                if (_SymbolMap.ContainsKey(vs.id))
                {
                    Debug.LogWarning($"{nameof(ReelView)} duplicate visual symbol id '{vs.id}'.", this);
                    continue;
                }

                _SymbolMap[vs.id] = vs.sprite;
            }
        }

        private void Advance()
        {
            if (_Disposed)
            {
                return;
            }

            if (string.IsNullOrEmpty(_Strip))
            {
                return;
            }

            _CurrentIndex = (_CurrentIndex + 1) % _Strip.Length;
            RenderCurrent();
        }

        private void RenderCurrent()
        {
            if (_Disposed)
            {
                return;
            }

            if (_Symbols == null || _Symbols.Length == 0)
            {
                return;
            }

            if (string.IsNullOrEmpty(_Strip))
            {
                return;
            }

            for (int i = 0; i < SlotDefinition.Rows && i < _Symbols.Length; i++)
            {
                if (_Symbols[i] == null)
                {
                    continue;
                }

                int index = (_CurrentIndex + i) % _Strip.Length;
                char symbolId = _Strip[index];

                if (_SymbolMap != null && _SymbolMap.TryGetValue(symbolId, out Sprite sprite))
                {
                    _Symbols[i].sprite = sprite;
                }
            }
        }

        private static int Mod(int x, int m)
        {
            int r = x % m;
            if (r < 0)
            {
                return r + m;
            }

            return r;
        }

        private void OnDestroy()
        {
            _Disposed = true;

            CancelSpinLoop();

            if (_StopCompletion != null)
            {
                _StopCompletion.TrySetCanceled();
            }
        }
    }
}