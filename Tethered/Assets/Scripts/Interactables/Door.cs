using DG.Tweening;
using Tethered.Player;
using UnityEngine;

namespace Tethered.Interactables
{
    public class Door : Interactable
    {

        [Header("Identifier")]
        [SerializeField] private int hash;

        private SpriteRenderer doorSprite;

        private float doorFadeDuration;
        private Tween fadeTween;

        public int Hash { get => hash; }

        protected override void Awake()
        {
            // Call the parent Awake
            base.Awake();

            // Get components
            doorSprite = GetComponent<SpriteRenderer>();

            // Set the door fade duration
            doorFadeDuration = 0.5f;
        }

        /// <summary>
        /// Try to open the Door
        /// </summary>
        public override void Interact(InteractController controller)
        {
            // Check the InteractController for the correct key
            controller.CheckKey(this);
        }

        /// <summary>
        /// Unlock the Door
        /// </summary>
        public void Unlock()
        {
            // Fade out the Interact Symbol
            HideInteractSymbol(true);

            // Fade out the door and set inactive
            FadeDoor(0f, doorFadeDuration, () => gameObject.SetActive(false));
        }

        /// <summary>
        /// Fade the Door using tweening
        /// </summary>
        private void FadeDoor(float endValue, float duration, TweenCallback onComplete)
        {
            // Kill the fade tween if it exists
            fadeTween?.Kill();

            // Fade the interact symbol
            fadeTween = doorSprite.DOFade(endValue, duration);

            // Exit case - no callback was given
            if (onComplete == null) return;

            // Add completion listeners
            fadeTween.onComplete += onComplete;
        }
    }
}