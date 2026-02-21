using Project.Slots.Domain.Configuration.Definitions;
using Project.Slots.Domain.Engine;
using Project.Slots.Presentation.Configuration;
using Project.Slots.Presentation.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Project.Slots.Presentation.Views
{
    /// <summary>
    /// Renders winning lines over the reels once the spin result has been resolved and reels have stopped.
    /// </summary>
    /// <remarks>
    /// This presenter:
    /// - Listens to <see cref="GameManager.OnSpinResolved"/> to cache the latest <see cref="SpinResult"/>,
    /// - Listens to <see cref="GameManager.OnReelsStopped"/> to start cycling through wins,
    /// - Draws a polyline overlay based on the configured pattern,
    /// - Displays the payout value for the currently shown win.
    ///
    /// Performance note:
    /// Line segments are pooled to avoid per-cycle Instantiate/Destroy calls.
    /// </remarks>
    public sealed class WinLinesPresenter : MonoBehaviour
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

        private readonly List<Image> _ActiveSegments = new List<Image>();
        private readonly Stack<Image> _SegmentPool = new Stack<Image>();

        private SpinResult _LastResult;
        private CancellationTokenSource _CTS;

        private Dictionary<int, string> _PatternMap;

        private void Awake()
        {
            BuildPatternMap();
            ClearVisuals();
        }

        private void OnEnable()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnSpinResolved += OnSpinResolved;
                GameManager.Instance.OnReelsStopped += OnReelsStopped;
            }
        }

        private void OnDisable()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnSpinResolved -= OnSpinResolved;
                GameManager.Instance.OnReelsStopped -= OnReelsStopped;
            }

            StopLoop();
        }

        private void OnDestroy()
        {
            StopLoop();
            DisposePool();
        }

        private void BuildPatternMap()
        {
            _PatternMap = new Dictionary<int, string>();

            if (_Configuration == null)
            {
                Debug.LogWarning($"{nameof(WinLinesPresenter)} has no configuration assigned.", this);
                return;
            }

            if (_Configuration.Patterns == null)
            {
                Debug.LogWarning($"{nameof(WinLinesPresenter)} configuration has null Patterns.", this);
                return;
            }

            _PatternMap = _Configuration.Patterns
                .Where(p => p != null)
                .GroupBy(p => p.id)
                .ToDictionary(g => g.Key, g => g.First().pattern);
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
                catch (OperationCanceledException)
                {
                    break;
                }
            }
        }

        private void ShowWin(WinLineDefinition win)
        {
            ClearLine();

            if (_PatternMap == null || !_PatternMap.TryGetValue(win.PatternId, out string pattern))
            {
                ClearPayout();
                return;
            }

            if (_Reels == null || _Reels.Length == 0)
            {
                ClearPayout();
                return;
            }

            string[] colPatterns = pattern.Split(',');
            int count = Mathf.Clamp(win.MatchCount, 2, _Reels.Length);

            List<Vector2> points = BuildPoints(colPatterns, count);
            if (points.Count < 2)
            {
                ClearPayout();
                return;
            }

            DrawPolyline(points);
            UpdatePayoutText(win, points);
        }

        private List<Vector2> BuildPoints(string[] colPatterns, int count)
        {
            List<Vector2> points = new List<Vector2>(count);

            for (int column = 0; column < count; column++)
            {
                if (column >= colPatterns.Length)
                {
                    break;
                }

                int row = colPatterns[column].IndexOf('1');
                if (row < 0)
                {
                    break;
                }

                Vector3 world = _Reels[column].GetSymbolWorldCenter(row);
                Vector2 local = WorldToWinRootLocal(world);
                points.Add(local);
            }

            return points;
        }

        private void UpdatePayoutText(WinLineDefinition win, List<Vector2> points)
        {
            if (_PayoutText == null)
            {
                return;
            }

            _PayoutText.text = $"{win.Payout}";

            RectTransform payoutRt = _PayoutText.rectTransform;
            payoutRt.anchoredPosition = GetPolylineMidpoint(points);
            payoutRt.localRotation = Quaternion.identity;

            payoutRt.SetAsLastSibling();
        }

        /// <summary>
        /// Computes the midpoint of a polyline by arc length.
        /// </summary>
        private Vector2 GetPolylineMidpoint(IReadOnlyList<Vector2> points)
        {
            float total = 0f;

            for (int i = 0; i < points.Count - 1; i++)
            {
                total += Vector2.Distance(points[i], points[i + 1]);
            }

            if (total <= 0.0001f)
            {
                return points[0];
            }

            float half = total * 0.5f;
            float acc = 0f;

            for (int i = 0; i < points.Count - 1; i++)
            {
                Vector2 a = points[i];
                Vector2 b = points[i + 1];

                float seg = Vector2.Distance(a, b);
                if (seg <= 0.0001f)
                {
                    continue;
                }

                if (acc + seg >= half)
                {
                    float t = (half - acc) / seg;
                    return Vector2.Lerp(a, b, t);
                }

                acc += seg;
            }

            return points[points.Count - 1];
        }

        private Vector2 WorldToWinRootLocal(Vector3 world)
        {
            if (_Canvas == null || _WinLinesRoot == null)
            {
                return Vector2.zero;
            }

            Vector2 screen = RectTransformUtility.WorldToScreenPoint(_Canvas.worldCamera, world);

            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                _WinLinesRoot,
                screen,
                _Canvas.worldCamera,
                out Vector2 local
            );

            return local;
        }

        private void DrawPolyline(List<Vector2> points)
        {
            for (int i = 0; i < points.Count - 1; i++)
            {
                CreateOrReuseSegment(points[i], points[i + 1]);
            }
        }

        private void CreateOrReuseSegment(Vector2 a, Vector2 b)
        {
            Image seg = AcquireSegment();
            if (seg == null)
            {
                return;
            }

            seg.raycastTarget = false;

            RectTransform rt = seg.rectTransform;

            Vector2 dir = b - a;
            float length = dir.magnitude;
            Vector2 mid = (a + b) * 0.5f;

            rt.anchoredPosition = mid;
            rt.sizeDelta = new Vector2(length, _Thickness);

            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            rt.localRotation = Quaternion.Euler(0, 0, angle);

            _ActiveSegments.Add(seg);

            if (_PayoutText != null)
            {
                _PayoutText.rectTransform.SetAsLastSibling();
            }
        }

        private Image AcquireSegment()
        {
            if (_WinLinesRoot == null || _LinePrefab == null)
            {
                return null;
            }

            Image seg;

            if (_SegmentPool.Count > 0)
            {
                seg = _SegmentPool.Pop();
            }
            else
            {
                seg = Instantiate(_LinePrefab, _WinLinesRoot);
            }

            seg.gameObject.SetActive(true);
            seg.transform.SetParent(_WinLinesRoot, false);
            return seg;
        }

        private void ReleaseSegment(Image seg)
        {
            if (seg == null)
            {
                return;
            }

            seg.gameObject.SetActive(false);
            _SegmentPool.Push(seg);
        }

        private void ClearLine()
        {
            for (int i = 0; i < _ActiveSegments.Count; i++)
            {
                ReleaseSegment(_ActiveSegments[i]);
            }

            _ActiveSegments.Clear();
        }

        private void ClearPayout()
        {
            if (_PayoutText != null)
            {
                _PayoutText.text = string.Empty;
            }
        }

        private void ClearVisuals()
        {
            ClearLine();
            ClearPayout();
        }

        private void DisposePool()
        {
            while (_SegmentPool.Count > 0)
            {
                Image seg = _SegmentPool.Pop();
                if (seg != null)
                {
                    Destroy(seg.gameObject);
                }
            }

            for (int i = 0; i < _ActiveSegments.Count; i++)
            {
                if (_ActiveSegments[i] != null)
                {
                    Destroy(_ActiveSegments[i].gameObject);
                }
            }

            _ActiveSegments.Clear();
        }
    }
}