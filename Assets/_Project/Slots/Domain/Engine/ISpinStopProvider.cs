namespace Project.Slots.Domain.Engine
{
    public interface ISpinStopProvider
    {
        bool TryGetStopIndexes(out int[] stopIndexes);
    }
}
