using DG.Tweening;
using System.Collections.Generic;
using Tethered.Player;
using UnityEngine;

namespace Tethered.Interactables
{
    [RequireComponent(typeof(BoxCollider2D))]
    public abstract class Interactable : MonoBehaviour
    {
        [SerializeField] protected bool sharedInteractable;
        [SerializeField] private bool multiSided;
        protected int enterDirection;
        protected HashSet<InteractController> controllers;

        protected bool symbolShown;
        protected float initialPosX;
        protected SpriteRenderer interactSymbol;

        private Tween fadeTween;
        protected float symbolFadeDuration;

        private Tween scaleTween;
        protected float scaleDuration;
        protected Vector2 symbolInitialScale;
        protected Vector2 symbolTargetScale;

        protected virtual void Awake()
        {
            // Get the sprite renderers in the children (includes the parent object)
            SpriteRenderer[] spriteRenderers = GetComponentsInChildren<SpriteRenderer>();

            // Get components
            interactSymbol = (!TryGetComponent(out SpriteRenderer renderer))
                ? spriteRenderers[0]
                : spriteRenderers[1];

            // Set the interact symbol's position
            Vector3 localPosition = interactSymbol.transform.localPosition;
            localPosition.x = Mathf.Abs(localPosition.x);
            interactSymbol.transform.localPosition = localPosition;

            // Set scale targets
            initialPosX = interactSymbol.transform.localPosition.x;
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
            // Exit case - if an InteractController is not found on the collision object
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

        protected virtual void OnTriggerExit2D(Collider2D collision)
        {
            // Exit case - if an InteractController is not found on the collision object
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
        /// Callback function that can be defined and called when an Interaction ends
        /// </summary>
        protected virtual void OnEnd()
        {
            // Noop
        }

        /// <summary>
        /// Show the interact symbol
        /// </summary>
        protected virtual void ShowInteractSymbol(bool notifyShown = false, TweenCallback onComplete = null)
        {
            // Check if multi-sided
            if(multiSided)
            {
                // Get the symbol's local position
                Vector3 localPosition = interactSymbol.transform.localPosition;

                // Switch the x-position based on the enter direction
                localPosition.x = initialPosX * Mathf.Sign(enterDirection);

                // Shift the symbol position to match the side
                interactSymbol.transform.localPosition = localPosition;
            }

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
            Scale(symbolTargetScale, scaleDuration, onComplete);
        }

        /// <summary>
        /// Hide the interact symbol
        /// </summary>
        protected virtual void HideInteractSymbol(bool notifyHidden = false, TweenCallback onComplete = null)
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
            Scale(symbolInitialScale, scaleDuration, onComplete);
        }

        /// <summary>
        /// Fade the Interact Symbol using Tweening
        /// </summary>
        protected void Fade(float endValue, float duration, TweenCallback onComplete = null)
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
        protected void Scale(Vector3 target, float duration, TweenCallback onComplete = null)
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