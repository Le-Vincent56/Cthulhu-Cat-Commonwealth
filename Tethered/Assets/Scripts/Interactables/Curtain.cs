using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Tethered.Player;
using UnityEngine;

namespace Tethered.Interactables
{
    public class Curtain : Interactable
    {
        private SpriteRenderer curtainSprite;

        private Tween fadeTween;
        private float curtainFadeDuration;

        protected override void Awake()
        {
            // Call the parent Awake
            base.Awake();

            // Get components
            curtainSprite = GetComponent<SpriteRenderer>();

            // Set the curtain fade duration
            curtainFadeDuration = 0.5f;
        }

        public override void Interact(InteractController controller)
        {
            // Store a curtain
            controller.Inventory.StoreCurtain();

            // Disable the curtain
            DisableCurtain();
        }

        /// <summary>
        /// Disable the Curtain
        /// </summary>
        private void DisableCurtain()
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

            // Fade out the key and disable the game object
            FadeCurtain(0f, curtainFadeDuration, () => gameObject.SetActive(false));
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