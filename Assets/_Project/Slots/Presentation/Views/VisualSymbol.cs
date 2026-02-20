using UnityEngine;

namespace Project.Slots.Presentation.Views
{
    [CreateAssetMenu(fileName = "VisualSymbol", menuName = "Scriptable Objects/VisualSymbol")]
    public class VisualSymbol : ScriptableObject
    {
        public char id;
        public Sprite sprite;
    }
}