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

        private Tween fadeTween;
        private float curtainFadeDuration;

        protected override void Awake()
        {
            base.Awake();

            // Set variables
            used = false;
            sharedInteractable = true;
            curtainFadeDuration = 0.5f;
        }

        protected override void OnTriggerEnter2D(Collider2D collision)
        {
            // Exit case - if the keyhole has already been used
            if (used) return;

            // Exit case - if a PlayerTwoController is not found on the collision object
            if (!collision.TryGetComponent(out InteractController controller)) return;

            // Set the controller's interactable
            controller.SetInteractable(this);

            // Add the controller to the hashset
            controllers.Add(controller);

            // Exit case - if the symbol is shown
            if (symbolShown) return;

            // Get the direction the controller is entering from
            enterDirection = (int)(controller.transform.position.x - transform.position.x);

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

        public override void Interact(InteractController controller)
        {
            if (controller.Inventory.CheckCurtain(this))
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

            // Hide the interact symbol
            HideInteractSymbol(true);

            used = false;
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