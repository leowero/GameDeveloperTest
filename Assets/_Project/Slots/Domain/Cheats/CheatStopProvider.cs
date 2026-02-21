using Project.Slots.Data;
using Project.Slots.Domain.Engine;
using System;
using System.Collections.Generic;

namespace Project.Slots.Domain.Cheats
{
    /// <summary>
    /// Stop provider that forces deterministic spins based on queued cheat requests.
    /// </summary>
    /// <remarks>
    /// This provider maintains an internal FIFO queue of cheat requests. Each successful call to
    /// <see cref="TryGetStopIndexes(out int[])"/> consumes one queued request and attempts to
    /// translate it into forced reel stop indexes using <c>CheatPlanner</c>.
    ///
    /// This class is intended for debugging and cheat workflows and is typically used on Unity's
    /// main thread.
    /// </remarks>
    public sealed class CheatStopProvider : ISpinStopProvider
    {
        private readonly IReadOnlyList<Pattern> _Patterns;
        private readonly Queue<CheatRequest> _Queue = new Queue<CheatRequest>();

        /// <summary>
        /// Creates a new <see cref="CheatStopProvider"/>.
        /// </summary>
        /// <param name="patterns">
        /// Pattern definitions used by the cheat planner to construct forced outcomes.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="patterns"/> is <c>null</c>.
        /// </exception>
        public CheatStopProvider(IReadOnlyList<Pattern> patterns)
        {
            if (patterns == null)
            {
                throw new ArgumentNullException(nameof(patterns));
            }

            _Patterns = patterns;
        }

        /// <summary>
        /// Enqueues a cheat request to be consumed on a future spin.
        /// </summary>
        /// <param name="request">Cheat request to enqueue.</param>
        /// <remarks>
        /// Requests are processed in FIFO order. This method does not validate whether the request
        /// is satisfiable, that is handled when the request is consumed.
        /// </remarks>
        public void Queue(CheatRequest request)
        {
            _Queue.Enqueue(request);
        }

        /// <summary>
        /// Attempts to provide forced stop indexes for the next spin based on queued cheat requests.
        /// </summary>
        /// <param name="stopIndexes">
        /// Forced stop indexes for the next spin when available; otherwise <c>null</c>.
        /// </param>
        /// <returns>
        /// <c>true</c> when a queued request could be translated into forced stops, otherwise <c>false</c>.
        /// </returns>
        /// <remarks>
        /// If the internal queue is empty, this method returns <c>false</c>.
        /// If a request exists but cannot be fulfilled by the cheat planner, the request is consumed
        /// and the method returns <c>false</c>.
        /// </remarks>
        public bool TryGetStopIndexes(out int[] stopIndexes)
        {
            stopIndexes = null;

            if (_Queue.Count == 0)
            {
                return false;
            }

            CheatRequest request = _Queue.Dequeue();
            return CheatPlanner.TryBuildForcedStops(request, _Patterns, out stopIndexes);
        }
    }
}