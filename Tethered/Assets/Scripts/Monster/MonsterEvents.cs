using Tethered.Patterns.EventBus;

namespace Tethered.Monster.Events
{
    public struct IncreaseAttraction : IEvent
    {
        public float GainedAttraction;
    }
}