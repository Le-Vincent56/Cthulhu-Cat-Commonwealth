using DG.Tweening;
using Tethered.Monster.Events;
using Tethered.Patterns.EventBus;
using Tethered.Player;
using UnityEngine;

namespace Tethered.Interactables
{
    public class Window : Interactable
    {
        [Header("References")]
        [SerializeField] private SpriteRenderer curtainSprite;

        [Header("General")]
        [SerializeField] private int hash;
        [SerializeField] private bool used;
        private bool hasCurtain;

        private Tween fadeTween;
        private float curtainFadeDuration;

        protected override void Awake()
        {
            base.Awake();

            // Set variables
            used = false;
            curtainFadeDuration = 0.5f;
        }

        protected override void OnTriggerEnter2D(Collider2D collision)
        {
            // Exit case - if the keyhole has already been used
            if (used) return;

            // Exit case - if a PlayerTwoController is not found on the collision object
            if (!collision.TryGetComponent(out InteractController controller)) return;

            // Exit case - if the Player does not have a curtain
            if(!controller.Inventory.CheckCurtain())
            {
                // Set that the Player does not have the curtain
                hasCurtain = false;

                // Exit case - if the Item symbol is shown
                if (itemSymbolShown) return;

                // Get the direction the controller is entering from
                enterDirection = (int)(controller.transform.position.x - transform.position.x);

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

            // Exit case - if the symbol is shown
            if (interactSymbolShown) return;

            // Get the direction the controller is entering from
            enterDirection = (int)(controller.transform.position.x - transform.position.x);

            // Set that the Player has the curtain
            hasCurtain = true;

            // Set full item opacity
            currentItemOpacity = 1f;

            // Show the interact symbol and the item symbol
            ShowItemSymbol();
            ShowInteractSymbol();
        }

        protected void OnTriggerStay2D(Collider2D collision)
        {
            // Exit case - if the Widnow has already been used
            if (used) return;

            // Exit case - Curtain is already checked
            if (hasCurtain) return;

            // Exit case - if a PlayerTwoController is not found on the collision object
            if (!collision.TryGetComponent(out InteractController controller)) return;

            // Exit case - if the Player does not have a curtain
            if (!controller.Inventory.CheckCurtain())
            {
                // Set that the Player does not have the curtain
                hasCurtain = false;

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

            // Exit case - if the symbol is shown
            if (interactSymbolShown) return;

            // Get the direction the controller is entering from
            enterDirection = (int)(controller.transform.position.x - transform.position.x);

            // Set that the Player has the curtain
            hasCurtain = true;

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

        public override void Interact(InteractController controller)
        {
            // Exit case - the Player does not have a curtain
            if (!hasCurtain)
            {
                // Shake the Item symbol
                ShakeItemSymbol();

                return;
            }

            // Cover the window
            CoverWindow();

            // Remove the Curtain from the inventory
            controller.Inventory.RemoveCurtain();

            // Disable the Window
            DisableWindow();
        }

        /// <summary>
        /// Disable the keyhole
        /// </summary>
        private void DisableWindow()
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

            // Play the SFX
            sfxManager.CreateSound().WithSoundData(soundData).Play();

            // Hide the Interact symbol and the Item symbol
            HideInteractSymbol();
            HideItemSymbol();

            used = true;
        }

        /// <summary>
        /// Cover the Window and disabled the Trigger
        /// </summary>
        public void CoverWindow()
        {
            // Fade in the curtain
            FadeCurtain(1f, curtainFadeDuration);

            // Disable the trigger
            EventBus<ToggleTrigger>.Raise(new ToggleTrigger() { Hash = hash, Enabled = false });

            // Set to use
            used = true;
        }

        /// <summary>
        /// Fade the Curtain using tweening
        /// </summary>
        private void FadeCurtain(float endValue, float duration, TweenCallback onComplete = null)
        {
            // Kill the fade tween if it exists
            fadeTween?.Kill();

            // Fade the interact symbol
            fadeTween = curtainSprite.DOFade(endValue, duration);

            // Exit case - no callback was given
            if (onComplete == null) return;

            // Add completion listeners
            fadeTween.onComplete += onComplete;
        }
    }
}