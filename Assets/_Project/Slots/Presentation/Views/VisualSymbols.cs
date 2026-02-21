using Project.Slots.Domain.Engine;
using Project.Slots.Presentation.Controllers;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Project.Slots.Presentation.Views
{
    /// <summary>
    /// Drives the visual reel spinning and stopping sequence in response to spin results.
    /// </summary>
    /// <remarks>
    /// This view listens to <see cref="GameManager.OnSpinResolved"/> and animates all reels to the
    /// provided stop indexes. Once the animation completes, it notifies the game manager through
    /// <see cref="GameManager.NotifyReelsStopped"/>.
    ///
    /// Timing note:
    /// The configured min/max spin durations represent the intended total duration of the full
    /// sequence, including start and stop staggering.
    /// </remarks>
    public sealed class VisualSymbols : MonoBehaviour
    {
        [SerializeField] private ReelView[] _Reels;

        [Header("Timing")]
        [SerializeField] private int _MinSpinDurationMs = 2000;
        [SerializeField] private int _MaxSpinDurationMs = 4000;
        [SerializeField] private int _DelayMs = 200;

        private bool _Animating;
        private CancellationTokenSource _Cts;

        private void Awake()
        {
            if (_Reels == null || _Reels.Length == 0)
            {
                Debug.LogWarning($"{nameof(VisualSymbols)} has no reels assigned.", this);
            }

            if (_MinSpinDurationMs < 0)
            {
                _MinSpinDurationMs = 0;
            }

            if (_MaxSpinDurationMs < _MinSpinDurationMs)
            {
                _MaxSpinDurationMs = _MinSpinDurationMs;
            }

            if (_DelayMs < 0)
            {
                _DelayMs = 0;
            }
        }

        private void OnEnable()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnSpinResolved += HandleSpinResolved;
            }
        }

        private void OnDisable()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnSpinResolved -= HandleSpinResolved;
            }
        }

        private void OnDestroy()
        {
            if (_Cts != null)
            {
                _Cts.Cancel();
                _Cts.Dispose();
                _Cts = null;
            }
        }

        private async void HandleSpinResolved(SpinResult result)
        {
            if (_Animating)
            {
                return;
            }

            if (result == null)
            {
                return;
            }

            _Animating = true;

            try
            {
                if (_Cts != null)
                {
                    _Cts.Cancel();
                    _Cts.Dispose();
                }

                _Cts = new CancellationTokenSource();

                await AnimateReels(result.StopIndexes, _Cts.Token);

                if (GameManager.Instance != null)
                {
                    GameManager.Instance.NotifyReelsStopped();
                }
            }
            catch (OperationCanceledException)
            {
                // Ignore cancellations (object destroyed/disabled or new spin overrides).
            }
            finally
            {
                _Animating = false;
            }
        }

        /// <summary>
        /// Animates all reels with a total duration in the configured min/max range.
        /// </summary>
        private async Task AnimateReels(IReadOnlyList<int> stopIndexes, CancellationToken token)
        {
            if (_Reels == null || _Reels.Length == 0)
            {
                return;
            }

            if (stopIndexes == null || stopIndexes.Count < _Reels.Length)
            {
                Debug.LogWarning($"{nameof(VisualSymbols)} received invalid stop indexes.", this);
                return;
            }

            int reelsCount = _Reels.Length;

            int startStaggerMs = _DelayMs * Mathf.Max(0, reelsCount - 1);
            int stopStaggerMs = _DelayMs * Mathf.Max(0, reelsCount - 1);

            int targetTotalMs = UnityEngine.Random.Range(_MinSpinDurationMs, _MaxSpinDurationMs + 1);
            int holdMs = Mathf.Max(0, targetTotalMs - startStaggerMs - stopStaggerMs);

            // Start reels (staggered), do not await each start to avoid serial time accumulation.
            for (int i = 0; i < reelsCount; i++)
            {
                _ = _Reels[i].StartSpin();
                await Task.Delay(_DelayMs, token);
            }

            // Hold spin so total stays within target window.
            if (holdMs > 0)
            {
                await Task.Delay(holdMs, token);
            }

            // Stop reels with stagger, but await all stops together (no serial accumulation).
            List<Task> stopTasks = new List<Task>(reelsCount);

            for (int i = 0; i < reelsCount; i++)
            {
                int reelIndex = i;
                stopTasks.Add(StopReelWithDelay(reelIndex, stopIndexes[reelIndex], token));
            }

            await Task.WhenAll(stopTasks);
        }

        private async Task StopReelWithDelay(int reelIndex, int stopIndex, CancellationToken token)
        {
            int delayBeforeStop = _DelayMs * reelIndex;

            if (delayBeforeStop > 0)
            {
                await Task.Delay(delayBeforeStop, token);
            }

            await _Reels[reelIndex].StopSpin(stopIndex);
        }
    }
}