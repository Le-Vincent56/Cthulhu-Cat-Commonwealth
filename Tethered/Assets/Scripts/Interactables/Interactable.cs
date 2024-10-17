using DG.Tweening;
using System.Collections.Generic;
using Tethered.Player;
using UnityEngine;

namespace Tethered.Interactables
{
    [RequireComponent(typeof(BoxCollider2D))]
    public abstract class Interactable : MonoBehaviour
    {
        [SerializeField] private bool sharedInteractable;
        protected HashSet<InteractController> controllers;

        protected bool symbolShown;
        protected SpriteRenderer interactSymbol;

        private Tween fadeTween;
        private float symbolFadeDuration;

        private Tween scaleTween;
        private float scaleDuration;
        private Vector2 symbolInitialScale;
        private Vector2 symbolTargetScale;

        protected virtual void Awake()
        {
            // Get the sprite renderers in the children (includes the parent object)
            SpriteRenderer[] spriteRenderers = GetComponentsInChildren<SpriteRenderer>();

            // Get components
            interactSymbol = (!TryGetComponent(out SpriteRenderer renderer))
                ? spriteRenderers[0]
                : spriteRenderers[1];

            // Set scale targets
            symbolInitialScale = interactSymbol.transform.localScale * 0.7f;
            symbolTargetScale = interactSymbol.transform.localScale;

            // Set animation durations
            symbolFadeDuration = 0.3f;
            scaleDuration = 0.5f;

            // Initialize the hashset
            controllers = new HashSet<InteractController>();

            // Hide the interact symbol
            Fade(0f, 0f);
            Scale(symbolInitialScale, 0f);
        }

        protected virtual void OnTriggerEnter2D(Collider2D collision)
        {
            // Exit case - if a PlayerTwoController is not found on the collision object
            if (!collision.TryGetComponent(out InteractController controller)) return;

            // Set the controller's interactable
            controller.SetInteractable(this);

            // Add the controller to the hashset
            controllers.Add(controller);

            // Exit case - if the symbol is shown
            if (symbolShown) return;

            // Show the interact symbol
            ShowInteractSymbol(sharedInteractable);
        }

        protected virtual void OnTriggerExit2D(Collider2D collision)
        {
            // Exit case - if a PlayerTwoController is not found on the collision object
            if (!collision.TryGetComponent(out InteractController controller)) return;

            // Set the controller's interactable
            controller.SetInteractable(null);

            // Exit case - if not a shared interactable
            if(!sharedInteractable)
            {
                // Hide the interact symbol
                HideInteractSymbol(sharedInteractable);
            }

            // Remove the controller from the hashset
            controllers.Remove(controller);

            // Hide the interact symbol if there are no present controllers
            if(controllers.Count <= 0)
                HideInteractSymbol(sharedInteractable);
        }

        /// <summary>
        /// Interact with the Interactable
        /// </summary>
        public abstract void Interact(InteractController controller);

        /// <summary>
        /// Show the interact symbol
        /// </summary>
        protected virtual void ShowInteractSymbol(bool notifyShown = false)
        {
            // Check if to notify shown
            if(notifyShown)
            {
                // Fade in and notify that the symbol is shown
                Fade(1f, symbolFadeDuration , () => symbolShown = true);
            } else
            {
                // Fade in
                Fade(1f, symbolFadeDuration);
            }

            // Scale to target
            Scale(symbolTargetScale, scaleDuration);
        }

        /// <summary>
        /// Hide the interact symbol
        /// </summary>
        protected virtual void HideInteractSymbol(bool notifyHidden = false)
        {
            // Check if to notify hidden
            if(notifyHidden)
            {
                // Fade out and notify that the symbol is hidden
                Fade(0f, symbolFadeDuration, () => symbolShown = false);
            } else
            {
                // Fade out
                Fade(0f, symbolFadeDuration);
            }

            // Scale to initial
            Scale(symbolInitialScale, scaleDuration);
        }

        /// <summary>
        /// Fade the Interact Symbol using Tweening
        /// </summary>
        private void Fade(float endValue, float duration, TweenCallback onComplete = null)
        {
            // Kill the fade tween if it exists
            fadeTween?.Kill();

            // Fade the interact symbol
            fadeTween = interactSymbol.DOFade(endValue, duration);

            // Exit case - no callback was given
            if (onComplete == null) return;

            // Add completion listeners
            fadeTween.onComplete += onComplete;
        }

        /// <summary>
        /// Scale the Interact Symbol using Tweening
        /// </summary>
        private void Scale(Vector3 target, float duration, TweenCallback onComplete = null)
        {
            // Kill the scale tween if it exists
            scaleTween?.Kill();

            // Scale the interact symbol
            scaleTween = interactSymbol.transform.DOScale(target, duration)
                    .SetEase(Ease.OutBounce);

            // Exit case - no callback was given
            if (onComplete == null) return;

            // Add completion listeners
            scaleTween.onComplete += onComplete;
        }
    }

}