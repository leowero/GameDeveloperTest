using UnityEngine;

namespace Project.Slots.Data
{
    [CreateAssetMenu(fileName = "Pattern", menuName = "Scriptable Objects/Pattern")]
    public class Pattern : ScriptableObject
    {
        public int id;
        public string pattern;
    }
}
