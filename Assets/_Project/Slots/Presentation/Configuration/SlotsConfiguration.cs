using Project.Slots.Data;
using System.Collections.Generic;
using UnityEngine;

namespace Project.Slots.Presentation.Configuration
{
    public class SlotsConfiguration : MonoBehaviour
    {
        [SerializeField] private List<Pattern> _Patterns;
        public IReadOnlyList<Pattern> Patterns => _Patterns;
    }
}
