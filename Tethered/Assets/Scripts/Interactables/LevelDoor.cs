using Tethered.Interactables;
using Tethered.Interactables.Events;
using Tethered.Patterns.EventBus;
using Tethered.Player;
using UnityEngine;

public class LevelDoor : Interactable
{
    [SerializeField] private int levelIndex;

    public override void Interact(InteractController controller)
    {
        EventBus<EndLevel>.Raise(new EndLevel() { LevelIndex = levelIndex });
    }
}
