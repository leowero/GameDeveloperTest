using UnityEngine;

namespace Project.Slots.Data
{
    /// <summary>
    /// Defines a win pattern used to evaluate symbol matches across the slot grid.
    /// </summary>
    /// <remarks>
    /// A pattern describes which row is evaluated for each column during win compilation.
    /// The <see cref="pattern"/> string is a comma-separated sequence of column descriptors,
    /// where each descriptor is a string of '0' and '1' characters.
    ///
    /// Each descriptor must contain exactly one '1', whose index represents the row to be
    /// evaluated for that column.
    ///
    /// Example for a 5x3 slot:
    /// <code>
    /// 100,100,100,100,100
    /// </code>
    /// This represents a horizontal payline on the top row (row 0) across all columns.
    /// </remarks>
    [CreateAssetMenu(fileName = "Pattern", menuName = "Scriptable Objects/Pattern")]
    public class Pattern : ScriptableObject
    {
        /// <summary>
        /// Unique identifier for this pattern.
        /// </summary>
        /// <remarks>
        /// Used to reference the pattern in win definitions.
        /// </remarks>
        public int id;

        /// <summary>
        /// Raw pattern definition string used by the slot engine.
        /// </summary>
        /// <remarks>
        /// Format: comma-separated column descriptors composed of '0' and '1' characters.
        /// Each descriptor must have a length equal to the number of rows in the slot grid
        /// and contain exactly one '1', indicating the row to evaluate for that column.
        ///
        /// Invalid formats may result in runtime errors or incorrect win evaluation.
        /// Validation of this format is expected to be handled by tooling or content pipelines.
        /// </remarks>
        public string pattern;
    }
}