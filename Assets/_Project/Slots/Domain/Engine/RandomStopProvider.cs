namespace Project.Slots.Domain.Engine
{
    public sealed class RandomStopProvider : ISpinStopProvider
    {
        public bool TryGetStopIndexes(out int[] stopIndexes)
        {
            stopIndexes = null;
            return false;
        }
    }
}