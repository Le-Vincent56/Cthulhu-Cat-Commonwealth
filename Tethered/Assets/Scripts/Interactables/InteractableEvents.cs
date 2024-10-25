using Tethered.Patterns.EventBus;

namespace Tethered.Interactables.Events
{
    public struct HandleDoor : IEvent 
    {
        public int Hash;
        public bool Open;
        public bool Deactivate;
    }
}