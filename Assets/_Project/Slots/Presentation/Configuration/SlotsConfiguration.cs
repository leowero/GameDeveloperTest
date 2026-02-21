using Project.Slots.Data;
using System.Collections.Generic;
using UnityEngine;

namespace Project.Slots.Presentation.Configuration
{
    /// <summary>
    /// Holds presentation-time slot configuration used to initialize gameplay systems.
    /// </summary>
    /// <remarks>
    /// This component is typically placed in the scene and referenced by controllers such as
    /// <see cref="Controllers.GameManager"/>.
    /// It currently exposes the set of win <see cref="Pattern"/> definitions used by the engine
    /// and cheat planner.
    /// </remarks>
    public sealed class SlotsConfiguration : MonoBehaviour
    {
        [SerializeField] private List<Pattern> _Patterns;

        /// <summary>
        /// Win pattern definitions used by the engine and cheat systems.
        /// </summary>
        /// <remarks>
        /// This collection may be empty, but many systems assume at least one valid pattern exists.
        /// </remarks>
        public IReadOnlyList<Pattern> Patterns => _Patterns;

        private void Awake()
        {
            if (_Patterns == null || _Patterns.Count == 0)
            {
                Debug.LogWarning($"{nameof(SlotsConfiguration)} has no patterns configured.", this);
            }
        }
    }
}