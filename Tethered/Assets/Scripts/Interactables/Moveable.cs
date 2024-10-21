using DG.Tweening;
using System.Collections.Generic;
using System.Linq;
using Tethered.Player;
using UnityEngine;
using UnityEngine.InputSystem.XR;

namespace Tethered.Interactables
{
    public class Moveable : Interactable
    {
        [Header("Components")]
        private Rigidbody2D rb;
        [SerializeField] private BoxCollider2D placementCollider;
        private Bounds colliderBounds;

        [Header("Interaction")]
        [SerializeField] private int numofPlayersRequired;
        [SerializeField] private bool canMove;
        private HashSet<MoveableController> moveableControllers;

        [Header("Tweening Variables")]
        [SerializeField] private float translateDuration;
        private Tween translateTween;

        protected override void Awake()
        {
            // Call parent Awake
            base.Awake();

            // Get components
            rb = GetComponent<Rigidbody2D>();
            placementCollider = GetComponents<BoxCollider2D>()[1];

            // Set variables
            canMove = true;
            colliderBounds = placementCollider.bounds;

            // Initialize the MoveableControllers HashSet
            moveableControllers = new();
        }

        /// <summary>
        /// Handle interaction with the Moveable
        /// </summary>
        public override void Interact(InteractController controller)
        {
            // Exit case - the Moveable cannot be moved
            if (!canMove) return;

            // Exit case - the MoveableControllers HashSet is not initialized
            if (moveableControllers == null) return;

            // Exit case - the controller does not have a PlayerController component
            if (!controller.TryGetComponent(out MoveableController moveableController)) return;

            // Set to kinematic and release constraints
            rb.isKinematic = true;
            rb.constraints = RigidbodyConstraints2D.None;

            // Check whether the HashSet already contains the MoveableController
            if(moveableControllers.Contains(moveableController))
            {
                // Otherwise, Set the player's object to move to null and notify to stop
                // moving an object
                moveableController.SetMovingObject(null, false);

                // Remove the MoveableController from the HashSet
                moveableControllers.Remove(moveableController);
            }
            else
            {
                // If not, set this as the player's object to move and notify
                // to start moving an object
                moveableController.SetMovingObject(this, true);

                // Add the MoveableController from the HashSet
                moveableControllers.Add(moveableController);
            }

            // Check if there are MoveableControllers
            if(moveableControllers.Count > 0)
            {
                // Hide the interact symbol
                HideInteractSymbol(false);
            }
            else if (moveableControllers.Count <= 0)
            {
                // Show the interact symbol
                ShowInteractSymbol(false);
            }
        }

        /// <summary>
        /// Move the Moveable
        /// </summary>
        public void Move(float moveSpeed)
        {
            // Exit case - there is no attached Player or the HashSet is not initialized
            if (moveableControllers == null || moveableControllers.Count <= 0) return;

            // Exit case - there's not enough Players to move the Moveable
            if (moveableControllers.Count < numofPlayersRequired) return;

            // Create a container for the final movement direction
            float xInputDirection = 0;

            // Iterate through each MoveableController
            foreach(MoveableController controller in moveableControllers)
            {
                // Accumulate inputs
                xInputDirection += controller.MoveInputX;
            }

            // Clamp the x-input direction to normalize
            xInputDirection = Mathf.Clamp(xInputDirection, -1, 1);

            // Set the Moveable's velocity
            rb.velocity = CalculateVelocity(xInputDirection, moveSpeed);

            // Iterate through each attached Player
            foreach (MoveableController controller in moveableControllers)
            {
                // Get collider bounds
                BoxCollider2D controllerCollider = controller.GetComponent<BoxCollider2D>();
                Bounds controllerBounds = controllerCollider.bounds;
                colliderBounds = placementCollider.bounds;

                // Calculate the distance between the Moveable and the Player
                Vector2 controllerPosition = controller.transform.position;
                Vector2 currentPosition = rb.position;

                // Calculate the horizontal direction
                float directionX = controllerPosition.x - currentPosition.x;

                // Adjust the controller's X position based on which side it's on
                Vector2 newControllerPosition = controllerPosition;

                // Check if the MoveableController is on the left
                if (directionX < 0)
                {
                    // Otherweise, align its right edge with the left edge of the Moveable
                    newControllerPosition.x = colliderBounds.min.x - (controllerBounds.size.x / 2f);
                }
                else
                {
                    // If so, align its left edge with the right edge of the Moveable
                    newControllerPosition.x = colliderBounds.max.x + (controllerBounds.size.x / 2f);
                }

                // Update the MoveableController's position
                controller.transform.position = newControllerPosition;
            }
        }
        
        /// <summary>
        /// Calculate the velocity for the Moveable
        /// </summary>
        private Vector2 CalculateVelocity(float xInputForce, float moveSpeed)
        {
            // Calculate the max speed depending on the amount of players moving
            // the Moveable
            float maxSpeed = (moveableControllers.Count > numofPlayersRequired)
                ? moveSpeed * 1.5f
                : moveSpeed;

            // Calculate the velocity
            Vector2 velocity = new Vector2(xInputForce * maxSpeed, 0f);

            return velocity;
        }

        /// <summary>
        /// Handle locking into place
        /// </summary>
        public void LockIntoPlace(Vector3 position)
        {
            // Exit case - there is no attached Player or the HashSet is not initialized
            if (moveableControllers == null || moveableControllers.Count <= 0) return;

            // Zero out the velocity
            rb.velocity = Vector2.zero;

            // Iterate through each attached MoveableController
            foreach(MoveableController controller in moveableControllers)
            {
                // Detach the Moveable from the MoveableController
                controller.SetMovingObject(null, false);
            }

            // Get the new position
            Vector3 newPosition = new Vector3(
                position.x,
                transform.position.y,
                transform.position.z
            );

            // Move to the position
            Translate(newPosition, translateDuration);

            // Lock the Moveable
            canMove = false;
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