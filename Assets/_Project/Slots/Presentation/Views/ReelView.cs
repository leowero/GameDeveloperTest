using Project.Slots.Domain.Configuration.Definitions;
using Project.Slots.Domain.Reels;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Project.Slots.Presentation.Views
{
    public class ReelView : MonoBehaviour
    {
        [SerializeField] private Image[] _Symbols;
        [SerializeField] private VisualSymbol[] _VisualSymbols;
        [SerializeField] private int _ReelIndex;
        [SerializeField] private int _StopDurationMs = 350;
        [SerializeField] private int _MinStopSteps = 6;

        private Dictionary<char, Sprite> _SymbolMap;
        private TaskCompletionSource<bool> _SpinCompletion;
        private string _Strip;
        private int _CurrentIndex;
        private bool _Spinning;
        private bool _Disposed;

        private void Awake()
        {
            _Strip = ReelStrips.Reels[_ReelIndex];
            _SymbolMap = _VisualSymbols.ToDictionary(x => x.id, x => x.sprite);
        }
        
        public async Task StartSpin()
        {
            if (_Spinning)
            {
                return;
            }

            _Spinning = true;
            int startDelay = 50;

            while (_Spinning)
            {
                Advance();
                await Task.Delay(startDelay);
            }
        }
        
        public async Task StopSpin(int stopIndex)
        {
            _SpinCompletion = new TaskCompletionSource<bool>();
            int rawSteps = (stopIndex - _CurrentIndex) % _Strip.Length;
            
            if (rawSteps < 0)
            {
                rawSteps += _Strip.Length;
            }
            
            int steps = rawSteps;
            if (steps < _MinStopSteps)
            {
                steps += _Strip.Length;
            }
            
            int perStepDelay = Mathf.Max(10, _StopDurationMs / steps);

            int safety = _Strip.Length * 3;
            while (steps-- > 0 && safety-- > 0)
            {
                Advance();
                await Task.Delay(perStepDelay);
            }

            _CurrentIndex = stopIndex; RenderCurrent();
            _Spinning = false;
            _SpinCompletion.SetResult(true);
        }
        
        private void Advance()
        {
            if (_Disposed)
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
            
            for (int i = 0; i < SlotDefinition.Rows; i++)
            {
                if (_Symbols[i] == null)
                {
                    return;
                }
                int index = (_CurrentIndex + i) % _Strip.Length;
                char symbol = _Strip[index]; _Symbols[i].sprite = _SymbolMap[symbol];
            }
        }

        public Task WaitUntilStopped()
        {
            return _SpinCompletion?.Task ?? Task.CompletedTask;
        }

        public Vector3 GetSymbolWorldCenter(int row)
        {
            if (_Symbols == null || row < 0 || row >= _Symbols.Length || _Symbols[row] == null)
            {
                return transform.position;
            }

            RectTransform rt = _Symbols[row].rectTransform;
            Vector3 localCenter = rt.rect.center;
            return rt.TransformPoint(localCenter);
        }

        private void OnDestroy()
        {
            _Disposed = true;
        }
    }
}