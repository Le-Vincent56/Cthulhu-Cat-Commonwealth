using Tethered.Patterns.EventBus;

namespace Tethered.Monster.Events
{
    public struct IncreaseAttraction : IEvent
    {
        public float GainedAttraction;
    }

    public struct AttractionChanged : IEvent
    {
        public float AttractionLevelTotal;
        public float AttractionLevelMax;
        public float AttractionThreshold;
        public int AttractionTier;
    }

    public struct ToggleTrigger : IEvent
    {
        public int Hash;
        public bool Enabled;
    }
}