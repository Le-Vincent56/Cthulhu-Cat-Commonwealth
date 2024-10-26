using UnityEngine;
using Tethered.Patterns.StateMachine;
using Tethered.Player.States;
using Tethered.Cameras;
using Tethered.Patterns.ServiceLocator;
using System.Collections.Generic;

namespace Tethered.Player
{
    public enum PlayerType
    {
        Older,
        Younger
    }

    public enum PlayerWeight
    {
        Light = 0, 
        Heavy = 1
    }

    public abstract class PlayerController : MonoBehaviour
    {
        protected Rigidbody2D rb;
        protected BoxCollider2D boxCollider;
        protected Animator animator;
        protected StateMachine stateMachine;
        protected MoveableController moveableController;
        protected CameraBoundary cameraBoundary;
        private Transform skinTransform;

        [Header("Movement")]
        [SerializeField] protected float movementSpeed;
        [SerializeField] protected PlayerWeight weight;
        protected int moveDirectionX;

        [Header("Climbing")]
        [SerializeField] protected bool reaching;
        [SerializeField] protected bool climbing;
        [SerializeField] private int currentLadderPathIndex;
        [SerializeField] private List<Vector2> ladderPath;
        [SerializeField] private float initialGravityScale;

        public PlayerWeight Weight { get => weight; }

        protected virtual void Awake()
        {
            // Get components
            rb = GetComponent<Rigidbody2D>();
            boxCollider = GetComponent<BoxCollider2D>();
            animator = GetComponentInChildren<Animator>();
            moveableController = GetComponent<MoveableController>();
            skinTransform = animator.transform;

            // Set variables
            initialGravityScale = rb.gravityScale;

            // Initialize the state machine
            stateMachine = new StateMachine();

            // Create states
            IdleState idleState = new IdleState(this, animator);
            LocomotionState locomotionState = new LocomotionState(this, animator);
            ClimbState climbState = new ClimbState(this, animator);
            MovingObjectState pushState = new MovingObjectState(this, animator, moveableController);

            // Set up individual states
            SetupStates(idleState, locomotionState, climbState);

            // Define state transitions
            stateMachine.At(idleState, locomotionState, new FuncPredicate(() => moveDirectionX != 0));
            stateMachine.At(idleState, pushState, new FuncPredicate(() => moveableController.MovingObject));

            stateMachine.At(locomotionState, idleState, new FuncPredicate(() => moveDirectionX == 0));
            stateMachine.At(idleState, climbState, new FuncPredicate(() => climbing));
            stateMachine.At(locomotionState, climbState, new FuncPredicate(() => climbing));
            stateMachine.At(climbState, idleState, new FuncPredicate(() => !climbing && moveDirectionX == 0));
            stateMachine.At(climbState, locomotionState, new FuncPredicate(() => !climbing && moveDirectionX != 0));
            stateMachine.At(locomotionState, pushState, new FuncPredicate(() => moveableController.MovingObject));

            stateMachine.At(pushState, idleState, new FuncPredicate(() => !moveableController.MovingObject && moveDirectionX == 0));
            stateMachine.At(pushState, locomotionState, new FuncPredicate(() => !moveableController.MovingObject && moveDirectionX != 0));

            // Set an initial state
            stateMachine.SetState(idleState);
        }

        protected virtual void Start()
        {
            // Retrieve the Camera boundary and register to it
            cameraBoundary = ServiceLocator.ForSceneOf(this).Get<CameraBoundary>();
            cameraBoundary.Register(this);
        }

        protected virtual void Update()
        {
            // Update the state machine
            stateMachine.Update();
        }

        protected virtual void FixedUpdate()
        {
            // Fixed update the state machine
            stateMachine.FixedUpdate();
        }

        /// <summary>
        /// Setup necessary states
        /// </summary>
        protected abstract void SetupStates(IdleState idleState, LocomotionState locomotionState, ClimbState climbState);

        /// <summary>
        /// Enable the input
        /// </summary>
        public abstract void EnableInput();

        /// <summary>
        /// Disable the input
        /// </summary>
        public abstract void DisableInput();

        /// <summary>
        /// Move the Player
        /// </summary>
        public void Move()
        {
            rb.velocity = new Vector2(moveDirectionX * movementSpeed, 0);
            SetFacingDirection(moveDirectionX);
        }

        /// <summary>
        /// Stop moving the Player
        /// </summary>
        public void EndMove() => rb.velocity = new Vector2(0, 0);

        /// <summary>
        /// Sets the facing direction of the player based on moveDirectionX.
        /// </summary>
        protected void SetFacingDirection(int direction)
        {
            // Check if direction is non-zero (player is moving)
            if (direction != 0)
            {
                // Flip the sprite based on the direction
                Vector3 scale = skinTransform.localScale;
                scale.x = Mathf.Abs(scale.x) * Mathf.Sign(direction);
                skinTransform.localScale = scale;
            }
        }

        /// <summary>
        /// Set whether or not the Player is reaching
        /// </summary>
        public void SetReaching(bool reaching) => this.reaching = reaching;

        /// <summary>
        /// Start climbing
        /// </summary>
        public void StartClimb(List<Vector2> path)
        {
            // Set crawling to true
            climbing = true;

            // Initialize the path
            ladderPath = path;
            currentLadderPathIndex = 1;
            boxCollider.enabled = false;
            rb.gravityScale = 0f;
        }

        /// <summary>
        /// Handle climb logic
        /// </summary>
        public void Climb()
        {
            // Exit case - if the path list is null
            if (ladderPath == null) return;

            // Exit case - if not climbing
            if (!climbing) return;

            // Get the current target point on the path
            Vector2 targetPoint = ladderPath[currentLadderPathIndex];

            // Calculate the direction towards the target point
            Vector2 currentPosition = rb.position;
            Vector2 direction = (targetPoint - currentPosition).normalized;

            // Calculate the step size based on the speed and time
            // TODO: CHANGE THIS 3 TO A VARIABLE!!
            float step = 3 * Time.fixedDeltaTime;

            // Move the player towards the target point
            rb.MovePosition(currentPosition + direction * step);

            // Check if the player has reached the target point
            if (Vector2.Distance(currentPosition, targetPoint) <= 0.1f)
            {
                // Move to the next point in the path
                currentLadderPathIndex++;

                // If we've reached the end of the path, stop climbing
                if (currentLadderPathIndex >= ladderPath.Count)
                {
                    EndClimb();
                }
            }
        }

        /// <summary>
        /// End climb
        /// </summary>
        public void EndClimb()
        {
            climbing = false;
            boxCollider.enabled = true;
            rb.gravityScale = initialGravityScale;
        }
    }
}
