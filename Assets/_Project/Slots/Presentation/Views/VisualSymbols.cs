using Project.Slots.Domain.Engine;
using Project.Slots.Presentation.Controllers;
using System.Threading.Tasks;
using UnityEngine;

namespace Project.Slots.Presentation.Views
{
    public class VisualSymbols : MonoBehaviour
    {
        [SerializeField] private ReelView[] _Reels;
        [SerializeField] private int _MinSpinDurationMs = 2000;
        [SerializeField] private int _MaxSpinDurationMs = 4000;
        [SerializeField] private int _Delay = 200;

        private bool _Animating;

        private void Start()
        {
            GameManager.Instance.OnSpinResolved += HandleSpin;
        }

        private void OnDestroy()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnSpinResolved -= HandleSpin;
            }
        }

        private async void HandleSpin(SpinResult result)
        {
            if (_Animating)
            {
                return;
            }
            _Animating = true;

            await AnimateReels(result.StopIndexes);
            GameManager.Instance.NotifyReelsStopped();

            _Animating = false;
        }

        private async Task AnimateReels(int[] stopIndexes)
        {
            for (int i = 0; i < _Reels.Length; i++)
            {
                _ = _Reels[i].StartSpin();
                await Task.Delay(_Delay);
            }

            int spinDuration = Random.Range(_MinSpinDurationMs, _MaxSpinDurationMs);
            await Task.Delay(spinDuration);

            for (int i = 0; i < _Reels.Length; i++)
            {
                await _Reels[i].StopSpin(stopIndexes[i]);
                await Task.Delay(_Delay);
            }
        }
    }
}
