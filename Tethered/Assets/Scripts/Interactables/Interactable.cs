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
        [SerializeField] private bool requiresItem;
        protected float currentItemOpacity;
        protected int enterDirection;
        protected HashSet<InteractController> controllers;

        protected SFXManager sfxManager;
        [SerializeField] protected SoundData soundData;

        protected bool interactSymbolShown;
        protected bool itemSymbolShown;

        protected float interactInitialPosX;
        protected float itemInitialPosX;
        protected SpriteRenderer interactSymbol;
        protected SpriteRenderer itemSymbol;

        private Tween fadeInteractTween;
        private Tween fadeItemTween;
        protected float symbolFadeDuration;

        private Tween scaleInteractTween;
        private Tween scaleItemTween;
        protected float scaleDuration;
        protected Vector2 interactInitialScale;
        protected Vector2 interactTargetScale;
        protected Vector2 itemInitialScale;
        protected Vector2 itemTargetScale;

        protected virtual void Awake()
        {
            // Get the sprite renderers in the children (includes the parent object)
            SpriteRenderer[] spriteRenderers = GetComponentsInChildren<SpriteRenderer>();

            // Check if there's a Sprite Renderer attached to the current object
            if(TryGetComponent(out SpriteRenderer renderer))
            {
                // Set Interact Item Symbol
                interactSymbol = spriteRenderers[1];

                // Check if an item is required to interact
                if (requiresItem)
                {
                    // Set the Item Sprite Renderer
                    itemSymbol = spriteRenderers[2];

                    // Set the current item opacity
                    currentItemOpacity = itemSymbol.color.a;

                    // Set the symbol to not shown
                    itemSymbolShown = false;
                }
            } 
            else
            {
                // Set Interact Sprite Renderer
                interactSymbol = spriteRenderers[0];

                // Check if an item is required to interact
                if (requiresItem)
                {
                    // Set the Item Sprite Renderer
                    itemSymbol = spriteRenderers[1];

                    // Set the current item opacity
                    currentItemOpacity = itemSymbol.color.a;

                    // Set the symbol to not shown
                    itemSymbolShown = false;
                }
            } 

            // Set the interact symbol's position
            Vector3 localPosition = interactSymbol.transform.localPosition;
            localPosition.x = Mathf.Abs(localPosition.x);
            interactSymbol.transform.localPosition = localPosition;

            // Set scale targets
            interactInitialPosX = interactSymbol.transform.localPosition.x;
            interactInitialScale = interactSymbol.transform.localScale * 0.7f;
            interactTargetScale = interactSymbol.transform.localScale;

            // Set animation durations
            symbolFadeDuration = 0.3f;
            scaleDuration = 0.5f;

            // Initialize the hashset
            controllers = new HashSet<InteractController>();

            // Hide the interact symbol
            Fade(0f, 0f);
            Scale(interactInitialScale, 0f);

            // Check if requiring an item
            if(requiresItem)
            {
                // Set the item symbol's position
                Vector3 itemLocalPosition = itemSymbol.transform.localPosition;
                itemLocalPosition.x = Mathf.Abs(itemLocalPosition.x);
                itemSymbol.transform.localPosition = itemLocalPosition;

                // Set item scale targets
                itemInitialPosX = itemSymbol.transform.localPosition.x;
                itemInitialScale = itemSymbol.transform.localScale * 0.7f;
                itemTargetScale = itemSymbol.transform.localScale;

                // Hide the item symbol
                FadeItem(0f, 0f);
                ScaleItem(itemInitialScale, 0f);
            }
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
            if (interactSymbolShown) return;

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
        /// Show the Interact symbol
        /// </summary>
        protected virtual void ShowInteractSymbol(TweenCallback onComplete = null)
        {
            // Check if multi-sided
            if(multiSided)
            {
                // Get the symbol's local position
                Vector3 localPosition = interactSymbol.transform.localPosition;

                // Switch the x-position based on the enter direction
                localPosition.x = interactInitialPosX * Mathf.Sign(enterDirection);

                // Shift the symbol position to match the side
                interactSymbol.transform.localPosition = localPosition;
            }

            // Fade in
            Fade(1f, symbolFadeDuration, () => interactSymbolShown = true);

            // Scale to target
            Scale(interactTargetScale, scaleDuration, onComplete);
        }

        /// <summary>
        /// Show the Item symbol
        /// </summary>
        /// <param name="onComplete"></param>
        protected virtual void ShowItemSymbol(TweenCallback onComplete = null)
        {
            // Check if multi-sided
            if (multiSided)
            {
                // Get the symbol's local position
                Vector3 localPosition = itemSymbol.transform.localPosition;

                // Switch the x-position based on the enter direction
                localPosition.x = itemInitialPosX * Mathf.Sign(enterDirection);

                // Shift the symbol position to match the side
                itemSymbol.transform.localPosition = localPosition;
            }

            // Fade in
            FadeItem(currentItemOpacity, symbolFadeDuration, () => itemSymbolShown = true);

            // Scale to target
            ScaleItem(itemTargetScale, scaleDuration, onComplete);
        }

        /// <summary>
        /// Update the Item symbol opacity
        /// </summary>
        protected virtual void UpdateItemOpacity(TweenCallback onComplete = null)
        {
            FadeItem(currentItemOpacity, symbolFadeDuration, () => itemSymbolShown = true);
        }

        /// <summary>
        /// Hide the Interact symbol
        /// </summary>
        protected virtual void HideInteractSymbol(TweenCallback onComplete = null)
        {
            // Fade out and notify that the symbol is hidden
            Fade(0f, symbolFadeDuration, () => interactSymbolShown = false);

            // Scale to initial
            Scale(interactInitialScale, scaleDuration, onComplete);
        }

        /// <summary>
        /// Hide the Item symbol
        /// </summary>
        protected virtual void HideItemSymbol(TweenCallback onComplete = null)
        {
            // Fade out and notify that the symbol is hidden
            FadeItem(0f, symbolFadeDuration, () => itemSymbolShown = false);

            // Scale to initial
            ScaleItem(itemInitialScale, scaleDuration, onComplete);
        }

        /// <summary>
        /// Fade the Interact Symbol using Tweening
        /// </summary>
        protected void Fade(float endValue, float duration, TweenCallback onComplete = null)
        {
            // Kill the Fade Interact Tween if it exists
            fadeInteractTween?.Kill();

            // Fade the interact symbol
            fadeInteractTween = interactSymbol.DOFade(endValue, duration);

            // Exit case - no callback was given
            if (onComplete == null) return;

            // Add completion listeners
            fadeInteractTween.onComplete += onComplete;
        }

        /// <summary>
        /// Fade the Item Symbol using Tweening
        /// </summary>
        protected void FadeItem(float endValue, float duration, TweenCallback onComplete = null)
        {
            // Kill the Fade Item Tween if it exists
            fadeItemTween?.Kill();

            // Fade the interact symbol
            fadeItemTween = itemSymbol.DOFade(endValue, duration);

            // Exit case - no callback was given
            if (onComplete == null) return;

            // Add completion listeners
            fadeItemTween.onComplete += onComplete;
        }

        /// <summary>
        /// Scale the Interact Symbol using Tweening
        /// </summary>
        protected void Scale(Vector3 target, float duration, TweenCallback onComplete = null)
        {
            // Kill the scale tween if it exists
            scaleInteractTween?.Kill();

            // Scale the interact symbol
            scaleInteractTween = interactSymbol.transform.DOScale(target, duration)
                    .SetEase(Ease.OutBounce);

            // Exit case - no callback was given
            if (onComplete == null) return;

            // Add completion listeners
            scaleInteractTween.onComplete += onComplete;
        }

        /// <summary>
        /// Scale the Item Symbol using Tweening
        /// </summary>
        protected void ScaleItem(Vector3 target, float duration, TweenCallback onComplete = null)
        {
            // Kill the scale tween if it exists
            scaleItemTween?.Kill();

            // Scale the interact symbol
            scaleItemTween = itemSymbol.transform.DOScale(target, duration)
                    .SetEase(Ease.OutBounce);

            // Exit case - no callback was given
            if (onComplete == null) return;

            // Add completion listeners
            scaleItemTween.onComplete += onComplete;
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