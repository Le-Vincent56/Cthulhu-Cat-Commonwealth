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
        }

        protected override void OnTriggerEnter2D(Collider2D collision)
        {
            // Exit case - if the keyhole has already been used
            if (used) return;

            // Exit case - if a PlayerTwoController is not found on the collision object
            if (!collision.TryGetComponent(out InteractController controller)) return;

            // Exit case - if the Player does not have the key
            if (!controller.Inventory.CheckKey(this))
            {
                // Set that the Player does not have the key
                hasKey = false;

                // Exit case - if the Item symbol is shown
                if (itemSymbolShown) return;

                // Show the Item symbol
                ShowItemSymbol();

                return;
            }

            // Set the controller's interactable
            controller.SetInteractable(this);

            // Add the controller to the hashset
            controllers.Add(controller);

            // Get the entering PlayerController
            PlayerController enteringPlayer = controller.GetComponent<PlayerController>();

            // Decide the sprite based on the entering player
            DecideEnterSprite(enteringPlayer);

            // Exit case - if the Interact symbol is shown
            if (interactSymbolShown) return;

            // Set that the Player has the key
            hasKey = true;

            // Set full item opacity
            currentItemOpacity = 1f;

            // Show the interact symbol and item symbol
            ShowItemSymbol();
            ShowInteractSymbol();
        }

        protected void OnTriggerStay2D(Collider2D collision)
        {
            // Exit case - if the keyhole has already been used
            if (used) return;

            // Exit case - already set to HasKey
            if (hasKey) return;

            // Exit case - if a PlayerTwoController is not found on the collision object
            if (!collision.TryGetComponent(out InteractController controller)) return;

            // Exit case - if the Player does not have the key, do not show the symbol
            if (!controller.Inventory.CheckKey(this))
            {
                // Set that the Player does not have the key
                hasKey = false;

                // Set the controller's interactable
                controller.SetInteractable(this);

                // Add the controller to the hashset
                controllers.Add(controller);

                // Exit case - if the Item symbol is shown
                if (itemSymbolShown) return;

                // Show the Item symbol
                ShowItemSymbol();

                return;
            }

            // Get the entering PlayerController
            PlayerController enteringPlayer = controller.GetComponent<PlayerController>();

            // Decide the sprite based on the entering player
            DecideEnterSprite(enteringPlayer);

            // Exit case - if the Interact symbol is shown
            if (interactSymbolShown) return;

            // Set that the Player has the key
            hasKey = true;

            // Set full item opacity
            currentItemOpacity = 1f;

            // Check if the Item symbol is shown
            if (itemSymbolShown)
                // If so, update the Item opacity
                UpdateItemOpacity();
            else
                // Otherwise, show the Item symbol
                ShowItemSymbol();

            // Show the Interact symbol
            ShowInteractSymbol();
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

            // Decide the sprite based on the remaining players in range
            DecideExitSprite();

            // Hide the interact symbol if there are no present controllers
            if (controllers.Count <= 0)
            {
                // Hide symbols
                HideInteractSymbol();
                HideItemSymbol();
            }
        }

        /// <summary>
        /// Try to open the attached Door
        /// </summary>
        public override void Interact(InteractController controller)
        {
            // Exit case - the Player does not have the key
            if (!hasKey)
            {
                // Shake the Item symbol
                ShakeItemSymbol();

                return;
            }

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
            HideInteractSymbol();

            // Hide the item symbol
            HideItemSymbol();

            // Set to used
            used = true;
        }
    }
}