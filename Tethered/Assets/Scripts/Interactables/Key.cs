using DG.Tweening;
using Tethered.Player;
using UnityEngine;

namespace Tethered.Interactables
{
    [RequireComponent(typeof(BoxCollider2D))]
    public class Key : Interactable
    {
        [SerializeField] private int hash;

        private SpriteRenderer keySprite;

        private Tween fadeTween;
        private float keyFadeDuration;
        
        protected override void Awake()
        {
            // Call the parent Awake
            base.Awake();

            // Get components
            keySprite = GetComponent<SpriteRenderer>();

            // Set the key fade duration
            keyFadeDuration = 0.5f;
        }

        protected override void ShowInteractSymbol(TweenCallback onComplete = null)
        {
            // Exit case - if the interact symbol is already shown
            if (symbolShown) return;

            // Call the parent function to handle tweening
            base.ShowInteractSymbol();
        }

        /// <summary>
        /// Pick up the Key
        /// </summary>
        public override void Interact(InteractController controller)
        {
            // Get the direction (for when setting Reach state)
            int direction = (int)Mathf.Sign(controller.transform.position.y - transform.position.y);

            // Check if the direction is from downward and if it is Player One
            if (direction == -1 && controller.TryGetComponent(out PlayerOneController playerController))
            {
                // Set reaching
                playerController.SetReaching(true);
            }

            // Add the key to the Players' inventory
            controller.Inventory.StoreKey(hash);

            // Disable the key
            DisableKey();
        }

        /// <summary>
        /// Disable the Key
        /// </summary>
        private void DisableKey()
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

            // Fade out the key and disable the game object
            FadeKey(0f, keyFadeDuration, () => gameObject.SetActive(false));
        }

        /// <summary>
        /// Set the Key's hash
        /// </summary>
        public void SetHash(int hash) => this.hash = hash;

        /// <summary>
        /// Fade the Key using tweening
        /// </summary>
        private void FadeKey(float endValue, float duration, TweenCallback onComplete = null)
        {
            // Kill the fade tween if it exists
            fadeTween?.Kill();

            // Fade the interact symbol
            fadeTween = keySprite.DOFade(endValue, duration);

            // Exit case - no callback was given
            if (onComplete == null) return;

            // Add completion listeners
            fadeTween.onComplete += onComplete;
        }
    }
}