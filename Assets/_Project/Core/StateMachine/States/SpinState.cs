using Project.Slots.Data;
using Project.Slots.Domain.Configuration.Definitions;
using Project.Slots.Domain.Engine;
using Project.Slots.Domain.Symbols;
using System;
using System.Collections.Generic;

namespace Project.Core.SlotMachine.States
{
    /// <summary>
    /// State responsible for executing a single spin and producing a <see cref="SpinResult"/>.
    /// </summary>
    /// <remarks>
    /// This state performs three main steps:
    /// - Resolves stop indexes, optionally overridden by an <see cref="ISpinStopProvider"/>,
    /// - Spins the engine to obtain the symbol grid and stop indexes,
    /// - Evaluates the grid against configured <see cref="Pattern"/> definitions to compile wins.
    /// </remarks>
    public class SpinState : IGameState
    {
        private readonly IReadOnlyList<Pattern> _Patterns;
        private readonly ISpinStopProvider _StopProvider;

        /// <summary>
        /// Creates a new spin state with the patterns used to evaluate wins and an optional stop provider.
        /// </summary>
        /// <param name="patterns">
        /// Patterns used by the engine to compile <see cref="WinLineDefinition"/> results for a given spin.
        /// </param>
        /// <param name="stopProvider">
        /// Optional provider that can force reel stop indexes, used for cheats and testing.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="patterns"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Thrown when <paramref name="patterns"/> is empty.
        /// </exception>
        /// <remarks>
        /// If <paramref name="stopProvider"/> is <c>null</c> or does not provide valid indexes,
        /// the engine will determine stop indexes normally.
        /// </remarks>
        public SpinState(IReadOnlyList<Pattern> patterns, ISpinStopProvider stopProvider)
        {
            if (patterns == null)
            {
                throw new ArgumentNullException(nameof(patterns));
            }

            if (patterns.Count == 0)
            {
                throw new ArgumentException("Patterns collection must not be empty.", nameof(patterns));
            }

            _Patterns = patterns;
            _StopProvider = stopProvider;
        }

        /// <summary>
        /// Called when the state becomes active.
        /// </summary>
        /// <remarks>
        /// Hook for spin preparation.
        /// The current implementation performs no operation.
        /// </remarks>
        public void Enter()
        {
            return;
        }

        /// <summary>
        /// Called when the state is exited.
        /// </summary>
        /// <remarks>
        /// Hook for cleanup.
        /// The current implementation performs no operation.
        /// </remarks>
        public void Exit()
        {
            return;
        }

        /// <summary>
        /// Executes the spin, evaluates wins, and returns a complete <see cref="SpinResult"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="SpinResult"/> containing the compiled winning lines and the final stop indexes.
        /// </returns>
        /// <remarks>
        /// If a stop provider is present and returns stop indexes, those indexes are validated
        /// before being passed into <see cref="SlotMachineEngine.Spin(out int[], int[])"/>.
        /// Invalid indexes are ignored and the engine performs a normal spin.
        /// </remarks>
        public SpinResult Action()
        {
            int[] forcedStopIndexes = TryGetValidForcedStops();

            SymbolType[][] grid = SlotMachineEngine.Spin(out int[] stopIndexes, forcedStopIndexes);
            IReadOnlyList<WinLineDefinition> wins = SlotMachineEngine.CompileWins(grid, _Patterns);

            return new SpinResult(wins, stopIndexes);
        }

        /// <summary>
        /// Attempts to obtain and validate forced stop indexes from the configured stop provider.
        /// </summary>
        /// <returns>
        /// A valid stop index array when available, otherwise <c>null</c>.
        /// </returns>
        /// <remarks>
        /// Validation performed here is intentionally conservative:
        /// - null arrays are rejected,
        /// - negative values are rejected,
        /// - length must match the engine reel count.
        /// Range validation beyond that depends on engine configuration and is not assumed here.
        /// </remarks>
        private int[] TryGetValidForcedStops()
        {
            if (_StopProvider == null)
            {
                return null;
            }

            if (!_StopProvider.TryGetStopIndexes(out int[] forcedIndexes) || forcedIndexes == null)
            {
                return null;
            }

            int expectedLength = SlotDefinition.Columns;

            if (forcedIndexes.Length != expectedLength)
            {
                return null;
            }

            for (int i = 0; i < forcedIndexes.Length; i++)
            {
                if (forcedIndexes[i] < 0)
                {
                    return null;
                }
            }

            return forcedIndexes;
        }
    }
}