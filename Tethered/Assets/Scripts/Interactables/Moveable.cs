using DG.Tweening;
using Tethered.Player;
using UnityEngine;

namespace Tethered.Interactables
{
    public class Moveable : Interactable
    {
        private Rigidbody2D rb;

        [Header("Interaction")]
        [SerializeField] private bool moving;

        [Header("Tweening Variables")]
        [SerializeField] private float translateDuration;
        private Tween translateTween;

        protected override void Awake()
        {
            // Call parent Awake
            base.Awake();

            // Get components
            rb = GetComponent<Rigidbody2D>();

            // Set moving to false
            moving = false;
        }

        /// <summary>
        /// Handle interaction with the Moveable
        /// </summary>
        public override void Interact(InteractController controller)
        {
            // Exit case - the controller does not have a PlayerController component
            if (!controller.TryGetComponent(out MoveableController moveableController)) return;

            // Check if already moving the Moveable
            if (!moving)
            {
                // If not, set this as the player's object to move and notify
                // to start moving an object
                moveableController.SetMovingObject(this, true);

                // Start moving the Moveable
                moving = true;

                // Hide the interact symbol
                HideInteractSymbol(false);
            }
            else
            {
                // Otherwise, Set the player's object to move to null and notify to stop
                // moving an object
                moveableController.SetMovingObject(null, false);

                // Stop moving the Moveable
                moving = false;

                // Show the interact symbol
                ShowInteractSymbol(false);
            }
        }

        /// <summary>
        /// Move the Moveable
        /// </summary>
        public void Move(Vector2 velocity) => rb.velocity = velocity;

        /// <summary>
        /// Handle locking into place
        /// </summary>
        public void LockIntoPlace(Vector3 position)
        {
            Translate(position, translateDuration);
        }
        
        /// <summary>
        /// Translate the Pushable
        /// </summary>
        private void Translate(Vector3 position, float duration, TweenCallback onComplete = null)
        {
            // Kill any existing translate tween
            translateTween?.Kill();

            // Translate the transform
            translateTween = transform.DOMove(position, duration);

            // Exit case - no callback function defined
            if (onComplete == null) return;

            // Set callbacks
            translateTween.onComplete += onComplete;
        }
    }
}