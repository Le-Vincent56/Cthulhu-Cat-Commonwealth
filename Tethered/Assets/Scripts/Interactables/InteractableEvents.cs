using Tethered.Patterns.EventBus;
using Tethered.Player;

namespace Tethered.Interactables.Events
{
    public struct HandleDoor : IEvent 
    {
        public int Hash;
        public bool Open;
        public bool Deactivate;
    }

    public struct DisableInteractables : IEvent
    {
        public int Hash;
    }

    public struct EnableLadder : IEvent
    {
        public int Hash;
    }

    public struct ActivateStairs : IEvent
    {
        public int Hash;
        public InteractController InteractController;
        public PlayerController PlayerController;
    }
}