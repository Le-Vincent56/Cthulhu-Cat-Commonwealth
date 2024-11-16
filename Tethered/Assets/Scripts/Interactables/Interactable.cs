using DG.Tweening;
using System.Collections.Generic;
using System.Linq;
using Tethered.Audio;
using Tethered.Patterns.ServiceLocator;
using Tethered.Player;
using UnityEngine;

namespace Tethered.Interactables
{
    [RequireComponent(typeof(BoxCollider2D))]
    public abstract class Interactable : MonoBehaviour
    {
        [SerializeField] protected bool playerTypeRestricted;
        [SerializeField] protected PlayerType allowedType;

        [SerializeField] protected Sprite olderHand;
        [SerializeField] protected Sprite youngerHand;
        [SerializeField] protected Sprite bothHands;
        [SerializeField] private bool multiSided;
        protected int enterDirection;
        protected HashSet<InteractController> controllers;

        protected SFXManager sfxManager;
        [SerializeField] protected SoundData soundData;

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

        protected virtual void Start()
        {
            sfxManager = ServiceLocator.ForSceneOf(this).Get<SFXManager>();
        }

        protected virtual void OnTriggerEnter2D(Collider2D collision)
        {
            // Exit case - if an InteractController is not found on the collision object
            if (!collision.TryGetComponent(out InteractController controller)) return;

            // Set the controller's interactable
            controller.SetInteractable(this);

            // Add the controller to the hashset
            controllers.Add(controller);

            // Get the entering player
            PlayerController enteringPlayer = controller.GetComponent<PlayerController>();

            // Decide the sprite based on the entering player
            DecideEnterSprite(enteringPlayer);

            // Exit case - if the symbol is shown
            if (symbolShown) return;

            // Check if the Interactable is restricted to a player type
            if (playerTypeRestricted)
            {
                // Exit case - based on which type is allowed
                if (enteringPlayer is PlayerOneController && allowedType != PlayerType.Older) return;
                if (enteringPlayer is PlayerTwoController && allowedType != PlayerType.Younger) return;
            }

            // Get the direction the controller is entering from
            enterDirection = (int)(controller.transform.position.x - transform.position.x);

            // Show the interact symbol
            ShowInteractSymbol();
        }

        protected virtual void OnTriggerExit2D(Collider2D collision)
        {
            // Exit case - if an InteractController is not found on the collision object
            if (!collision.TryGetComponent(out InteractController controller)) return;

            // Set the controller's interactable
            controller.SetInteractable(null);

            // Remove the controller from the hashset
            controllers.Remove(controller);

            // Decide the sprite based on the remaining players in range
            DecideExitSprite();

            // Hide the interact symbol if there are no present controllers
            if(controllers.Count <= 0)
                HideInteractSymbol();
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
        protected virtual void ShowInteractSymbol(TweenCallback onComplete = null)
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

            // Fade in
            Fade(1f, symbolFadeDuration, () => symbolShown = true);

            // Scale to target
            Scale(symbolTargetScale, scaleDuration, onComplete);
        }

        /// <summary>
        /// Hide the interact symbol
        /// </summary>
        protected virtual void HideInteractSymbol(TweenCallback onComplete = null)
        {
            // Fade out and notify that the symbol is hidden
            Fade(0f, symbolFadeDuration, () => symbolShown = false);

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

        protected void DecideEnterSprite(PlayerController enteringPlayer)
        {
            // Check if only a single player is within range
            if (controllers.Count < 2)
            {
                // Check if the entering player is the Older Sibling
                if (enteringPlayer is PlayerOneController)
                {
                    interactSymbol.sprite = olderHand;
                }
                // Check if the entering player is the Younger Sibling
                else if (enteringPlayer is PlayerTwoController)
                {
                    interactSymbol.sprite = youngerHand;
                }
            }
            // Check if both players are within the count
            else if (controllers.Count >= 2)
            {
                interactSymbol.sprite = bothHands;
            }
        }

        protected void DecideExitSprite()
        {
            // Exit case - there are no controllers currently in range
            if (controllers.Count <= 0) return;

            // Get the remaining player
            PlayerController remainingPlayer = controllers.ElementAt(0).GetComponent<PlayerController>();

            // Check if the entering player is the Older Sibling
            if (remainingPlayer is PlayerOneController)
            {
                interactSymbol.sprite = olderHand;
            }
            // Check if the entering player is the Younger Sibling
            else if (remainingPlayer is PlayerTwoController)
            {
                interactSymbol.sprite = youngerHand;
            }
        }
    }

}