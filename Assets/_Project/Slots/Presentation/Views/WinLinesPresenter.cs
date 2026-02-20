using Project.Slots.Domain.Configuration.Definitions;
using Project.Slots.Domain.Engine;
using Project.Slots.Presentation.Configuration;
using Project.Slots.Presentation.Controllers;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Project.Slots.Presentation.Views
{
    public class WinLinesPresenter : MonoBehaviour
    {
        [Header("Configuration")]
        [SerializeField] private Canvas _Canvas;
        [SerializeField] private SlotsConfiguration _Configuration;
        [SerializeField] private ReelView[] _Reels;

        [Header("UI")]
        [SerializeField] private RectTransform _WinLinesRoot;
        [SerializeField] private Image _LinePrefab;
        [SerializeField] private TMP_Text _PayoutText;

        [Header("Style")]
        [SerializeField] private float _Thickness = 12f;
        [SerializeField] private int _CycleMs = 1000;

        private readonly List<Image> _Segments = new();
        private SpinResult _LastResult;
        private CancellationTokenSource _CTS;

        private Dictionary<int, string> _PatternMap;

        private void Awake()
        {
            _PatternMap = _Configuration.Patterns.ToDictionary(p => p.id, p => p.pattern);
            ClearVisuals();
        }

        private void Start()
        {
            GameManager.Instance.OnSpinResolved += OnSpinResolved;
            GameManager.Instance.OnReelsStopped += OnReelsStopped;
        }

        private void OnDestroy()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnSpinResolved -= OnSpinResolved;
                GameManager.Instance.OnReelsStopped -= OnReelsStopped;
            }

            StopLoop();
        }

        private void OnSpinResolved(SpinResult result)
        {
            StopLoop();
            ClearVisuals();

            _LastResult = result;
        }

        private void OnReelsStopped()
        {
            if (_LastResult == null || _LastResult.Wins == null || _LastResult.Wins.Count == 0)
            {
                ClearVisuals();
                return;
            }

            StartLoop(_LastResult);
        }

        private void StartLoop(SpinResult result)
        {
            StopLoop();
            _CTS = new CancellationTokenSource();

            _ = LoopWins(result, _CTS.Token);
        }

        private void StopLoop()
        {
            if (_CTS != null)
            {
                _CTS.Cancel();
                _CTS.Dispose();
                _CTS = null;
            }
        }

        private async Task LoopWins(SpinResult result, CancellationToken ct)
        {
            int index = 0;

            while (!ct.IsCancellationRequested)
            {
                WinLineDefinition win = result.Wins[index];

                ShowWin(win);

                index++;
                if (index >= result.Wins.Count)
                {
                    index = 0;
                }

                try
                {
                    await Task.Delay(_CycleMs, ct);
                }
                catch
                {
                    break;
                }
            }
        }

        private void ShowWin(WinLineDefinition win)
        {
            ClearLine();

            if (!_PatternMap.TryGetValue(win.PatternId, out string pattern))
            {
                return;
            }

            string[] colPatterns = pattern.Split(',');

            int count = Mathf.Clamp(win.MatchCount, 2, _Reels.Length);
            var points = new List<Vector2>(count);

            for (int col = 0; col < count; col++)
            {
                int row = colPatterns[col].IndexOf('1');
                if (row < 0)
                {
                    break;
                }

                Vector3 world = _Reels[col].GetSymbolWorldCenter(row);
                Vector2 local = WorldToWinRootLocal(world);
                points.Add(local);
            }

            if (points.Count < 2)
            {
                return;
            }

            DrawPolyline(points);

            if (_PayoutText != null)
            {
                _PayoutText.text = $"{win.Payout}";
            }
        }

        private Vector2 WorldToWinRootLocal(Vector3 world)
        {
            Vector2 screen = RectTransformUtility.WorldToScreenPoint(_Canvas.worldCamera, world);

            RectTransformUtility.ScreenPointToLocalPointInRectangle(_WinLinesRoot, screen, _Canvas.worldCamera, out Vector2 local);

            return local;
        }

        private void DrawPolyline(List<Vector2> points)
        {
            for (int i = 0; i < points.Count - 1; i++)
            {
                CreateSegment(points[i], points[i + 1]);
            }
        }

        private void CreateSegment(Vector2 a, Vector2 b)
        {
            Image seg = Instantiate(_LinePrefab, _WinLinesRoot);
            seg.raycastTarget = false;

            RectTransform rt = seg.rectTransform;

            Vector2 dir = b - a;
            float length = dir.magnitude;
            Vector2 mid = (a + b) * 0.5f;

            rt.anchoredPosition = mid;
            rt.sizeDelta = new Vector2(length, _Thickness);

            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            rt.localRotation = Quaternion.Euler(0, 0, angle);

            _Segments.Add(seg);
        }

        private void ClearLine()
        {
            for (int i = 0; i < _Segments.Count; i++)
            {
                if (_Segments[i] != null)
                {
                    Destroy(_Segments[i].gameObject);
                } 
            }
            _Segments.Clear();
        }

        private void ClearVisuals()
        {
            ClearLine();

            if (_PayoutText != null)
            {
                _PayoutText.text = string.Empty;
            }
        }
    }
}