using DG.Tweening;
using Tethered.Input;
using Tethered.Interactables;
using UnityEngine;

namespace Tethered.Player
{
    public class MoveableController : MonoBehaviour
    {
        [SerializeField] private PlayerType playerType;
        [SerializeField] private PlayerOneInputReader playerOneInputReader;
        [SerializeField] private PlayerTwoInputReader playerTwoInputReader;

        private Rigidbody2D rb;
        private BoxCollider2D boxCollider;

        [Header("Moveables")]
        [SerializeField] protected bool movingObject;
        [SerializeField] private float moveSpeed;
        [SerializeField] private Moveable objectToMove;
        private float moveDirectionX;

        [Header("Tweening Variables")]
        [SerializeField] private float positionToMoveableDuration;
        private Tween translatePositionForMoveable;

        public bool MovingObject { get => movingObject; }

        private void Awake()
        {
            // Get components
            rb = GetComponent<Rigidbody2D>();
            boxCollider = GetComponent<BoxCollider2D>();
        }

        private void Update()
        {
            // Get the x-direction movement depending on the Player Type
            // and corresponding Input Readers
            switch (playerType)
            {
                case PlayerType.Older:
                    moveDirectionX = playerOneInputReader.NormMoveX;
                    break;
                case PlayerType.Younger:
                    moveDirectionX = playerTwoInputReader.NormMoveX;
                    break;
            }
        }

        /// <summary>
        /// Set whether or not the Player is moving an object
        /// </summary>
        public void SetMovingObject(Moveable objectToMove, bool isMoving)
        {
            this.objectToMove = objectToMove;
            movingObject = isMoving;
        }

        /// <summary>
        /// Position the Player to be next to the Moveable
        /// </summary>
        public void PositionWithMoveable(TweenCallback onComplete = null)
        {
            // Exit case - there is no Moveable
            if (objectToMove == null) return;

            // Kill the existing Tween
            translatePositionForMoveable?.Kill();

            // Get the Moveable's bounds
            Bounds moveableBounds = objectToMove.GetComponent<BoxCollider2D>().bounds;
            Bounds moveableSpriteBounds = objectToMove.GetComponent<SpriteRenderer>().bounds;

            // Get the player's bounds
            Bounds playerBounds = boxCollider.bounds;

            // Create a container for the target position
            Vector3 targetPosition;

            // Check which side of the Moveable the player is on
            if (transform.position.x < objectToMove.transform.position.x)
            {
                // If the Player is on the left side of the Moveable,
                // align the player with its left edge
                targetPosition = new Vector3(
                    moveableSpriteBounds.min.x - playerBounds.extents.x,
                    transform.position.y,
                    transform.position.z
                );
            }
            else
            {
                // If the player is on the right side of the Moveable,
                // align the Player with its right edge
                targetPosition = new Vector3(
                    moveableSpriteBounds.max.x + playerBounds.extents.x,
                    transform.position.y,
                    transform.position.z
                );
            }

            // Move the player
            translatePositionForMoveable = transform.DOMove(targetPosition, positionToMoveableDuration);

            // Exit case - no callback was defined
            if (onComplete == null) return;

            // Link callback functions
            translatePositionForMoveable.onComplete += onComplete;
        }

        public void MoveObject()
        {
            // Exit case - if there's no object to move
            if (objectToMove == null) return;

            Vector2 velocity = new Vector3(moveDirectionX * moveSpeed, 0);

            // Move the Player and the Moveable
            rb.velocity = velocity;
            objectToMove.Move(velocity);
        }
    }
}