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
        [SerializeField] private Transform spriteTransform;

        private Bounds playerBounds;
        private Bounds moveableBounds;
        private Bounds moveableSpriteBounds;

        [SerializeField] protected bool movingObject;
        [SerializeField] private float moveSpeed;
        [SerializeField] private Moveable objectToMove;
        private float directionOfMoveable;
        private int moveInputX;

        [SerializeField] private float positionToMoveableDuration;
        private Tween translatePositionForMoveable;

        public bool MovingObject { get => movingObject; }
        public int MoveInputX { get => moveInputX; }

        private void Awake()
        {
            // Get components
            rb = GetComponent<Rigidbody2D>();
            boxCollider = GetComponent<BoxCollider2D>();

            // Get the player's bounds
            playerBounds = boxCollider.bounds;
        }

        private void Update()
        {
            // Get the x-direction movement depending on the Player Type
            // and corresponding Input Readers
            switch (playerType)
            {
                case PlayerType.Older:
                    moveInputX = playerOneInputReader.NormMoveX;
                    break;
                case PlayerType.Younger:
                    moveInputX = playerTwoInputReader.NormMoveX;
                    break;
            }
        }

        /// <summary>
        /// Set whether or not the Player is moving an object
        /// </summary>
        public void SetMovingObject(Moveable objectToMove, bool isMoving)
        {
            // Set the object to move
            this.objectToMove = objectToMove;

            // Set state
            movingObject = isMoving;

            // Exit case - if the object to move is null
            if (objectToMove == null) return;

            // Get the direction of the moveable from the player
            directionOfMoveable = Mathf.Sign((objectToMove.transform.position - transform.position).normalized.x);

            // Get the Moveable's bounds
            moveableBounds = objectToMove.GetComponent<BoxCollider2D>().bounds;
            moveableSpriteBounds = objectToMove.GetComponent<SpriteRenderer>().bounds;
        }

        /// <summary>
        /// Position the Player to be next to the Moveable
        /// </summary>
        public void PositionWithMoveable(TweenCallback onComplete = null)
        {
            // Exit case - there is no Moveable
            if (objectToMove == null) return;

            Debug.Log($"Before: {spriteTransform.localScale}");

            // Face the player towards the Moveable
            Vector3 localScale = spriteTransform.localScale;
            localScale.x = directionOfMoveable;
            spriteTransform.localScale = localScale;

            Debug.Log($"After: {spriteTransform.localScale}");

            // Kill the existing Tween
            translatePositionForMoveable?.Kill();

            // Get the alignment position, aligning the player to the object
            Vector3 targetPosition = AlignObjectWithPlayer(true);

            // Move the player
            translatePositionForMoveable = transform.DOMove(targetPosition, positionToMoveableDuration);

            // Exit case - no callback was defined
            if (onComplete == null) return;

            // Link callback functions
            translatePositionForMoveable.onComplete += onComplete;
        }

        /// <summary>
        /// Move the Moveable Object
        /// </summary>
        public void MoveObject()
        {
            // Exit case - if there's no object to move
            if (objectToMove == null) return;

            // Move the Moveable
            objectToMove.Move(moveSpeed);
        }

        /// <summary>
        /// Set the velocity of the MoveableController
        /// </summary>
        public void SetVelocity(Vector2 velocity) => rb.velocity = velocity;

        /// <summary>
        /// Align the Moveable with the player
        /// </summary>
        /// <returns></returns>
        private Vector3 AlignObjectWithPlayer(bool alignPlayer)
        {
            // Create a container for the target position
            Vector3 targetPosition;

            // Set an offset
            float offset = 0f;

            // Set the target position
            targetPosition = new Vector3(
                        offset,
                        transform.position.y,
                        transform.position.z
                    );

            // Check whether to align the Player with the Moveable or
            // the Moveable with the player
            if (alignPlayer)
            {
                // Check which side of the Moveable the player is on
                if (transform.position.x < objectToMove.transform.position.x)
                {
                    // If the Player is on the left side of the Moveable,
                    // align the Player's right edge with the Moveable's left edge
                    targetPosition.x = moveableSpriteBounds.min.x - playerBounds.extents.x;
                }
                else
                {
                    // If the player is on the right side of the Moveable,
                    // align the Player's left edge with the Moveable's right edge
                    targetPosition.x = moveableSpriteBounds.max.x + playerBounds.extents.x;
                }
            }
            else
            {
                // Check which side of the Moveable the player is on
                if (transform.position.x < objectToMove.transform.position.x)
                {
                    // If the Player is on the left side of the Moveable,
                    // align the Moveable's left edge with the Player's right edge
                    targetPosition.x = playerBounds.max.x + moveableBounds.extents.x;
                }
                else
                {
                    // If the Player is on the right side of the Moveable,
                    // align the Moveable's right edge with the Player's left edge
                    targetPosition.x = playerBounds.min.x - moveableBounds.extents.x;
                }
            }

            return targetPosition;
        }
    }
}