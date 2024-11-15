using Tethered.Interactables.Events;
using Tethered.Patterns.EventBus;
using Tethered.Player;
using UnityEngine;

namespace Tethered.Interactables
{
    public class Keyhole : Interactable
    {
        [Header("Identifier")]
        [SerializeField] private int hash;
        private bool used;
        private bool hasKey;

        public int Hash { get => hash; }

        protected override void Awake()
        {
            base.Awake();

            used = false;
            sharedInteractable = true;
        }

        protected override void OnTriggerEnter2D(Collider2D collision)
        {
            // Exit case - if the keyhole has already been used
            if (used) return;

            // Exit case - if a PlayerTwoController is not found on the collision object
            if (!collision.TryGetComponent(out InteractController controller)) return;

            // Exit case - if the Player does not have the key, do not show the symbol
            if (!controller.Inventory.CheckKey(this))
            {
                hasKey = false;
                return;
            }

            // Set the controller's interactable
            controller.SetInteractable(this);

            // Add the controller to the hashset
            controllers.Add(controller);

            // Exit case - if the symbol is shown
            if (symbolShown) return;

            // Set that the Player has the key
            hasKey = true;

            // Show the interact symbol
            ShowInteractSymbol(sharedInteractable);
        }

        protected override void OnTriggerExit2D(Collider2D collision)
        {
            // Exit case - if the keyhole has already been used
            if (used) return;

            // Exit case - if a PlayerTwoController is not found on the collision object
            if (!collision.TryGetComponent(out InteractController controller)) return;

            // Set the controller's interactable to null
            controller.SetInteractable(null);

            // Remove the controller from the hashset
            controllers.Remove(controller);

            // Hide the interact symbol if there are no present controllers
            if (controllers.Count <= 0)
                HideInteractSymbol(sharedInteractable);
        }

        /// <summary>
        /// Try to open the attached Door
        /// </summary>
        public override void Interact(InteractController controller)
        {
            // Exit case - the Player does not have the key
            if (!hasKey) return;

            // Unlock the door
            EventBus<HandleDoor>.Raise(new HandleDoor()
            {
                Hash = hash,
                Open = true,
                Deactivate = true
            });

            // Remove the key
            controller.Inventory.RemoveKey(hash);

            // Play the sound
            sfxManager.CreateSound().WithSoundData(soundData).Play();

            // Disable the keyhole
            DisableKeyhole();
        }

        /// <summary>
        /// Disable the keyhole
        /// </summary>
        private void DisableKeyhole()
        {
            // Iterate through each controller within the trigger
            foreach (InteractController controller in controllers)
            {
                // Skip if the Player Controller does not have this as the current
                // interactable
                if (!controller.HasInteractable(this)) continue;

                // Set the interactable to null
                controller.SetInteractable(null);
            }

            // Hide the interact symbol
            HideInteractSymbol(true);

            used = false;
        }
    }
}