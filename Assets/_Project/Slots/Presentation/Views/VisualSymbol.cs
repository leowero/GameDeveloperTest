using UnityEngine;

namespace Project.Slots.Presentation.Views
{
    /// <summary>
    /// Maps a symbol identifier to its visual representation.
    /// </summary>
    /// <remarks>
    /// The <see cref="id"/> should match the raw symbol identifier used by reel strips and
    /// domain symbol mapping (e.g. 'C' for Cherry).
    /// </remarks>
    [CreateAssetMenu(fileName = "VisualSymbol", menuName = "Scriptable Objects/VisualSymbol")]
    public sealed class VisualSymbol : ScriptableObject
    {
        /// <summary>
        /// Raw symbol identifier used by the engine and reel strips.
        /// </summary>
        public char id;

        /// <summary>
        /// Sprite used to display this symbol in the UI.
        /// </summary>
        public Sprite sprite;
    }
}