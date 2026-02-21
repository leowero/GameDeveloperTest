using Project.Slots.Data;
using Project.Slots.Domain.Engine;
using System.Collections.Generic;

namespace Project.Slots.Domain.Cheats
{
    public sealed class CheatStopProvider : ISpinStopProvider
    {
        private readonly IReadOnlyList<Pattern> _Patterns;
        private readonly Queue<CheatRequest> _Queue = new Queue<CheatRequest>();

        public CheatStopProvider(IReadOnlyList<Pattern> patterns)
        {
            _Patterns = patterns;
        }

        public void Queue(CheatRequest request) => _Queue.Enqueue(request);

        public bool TryGetStopIndexes(out int[] stopIndexes)
        {
            stopIndexes = null;

            if (_Queue.Count == 0)
            {
                return false;
            }

            var request = _Queue.Dequeue();
            return CheatPlanner.TryBuildForcedStops(request, _Patterns, out stopIndexes);
        }
    }
}
