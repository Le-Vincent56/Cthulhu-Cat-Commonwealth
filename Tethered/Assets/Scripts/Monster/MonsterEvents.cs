using Tethered.Patterns.EventBus;
using UnityEngine.Serialization;

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
    }
}