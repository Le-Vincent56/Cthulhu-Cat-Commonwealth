using UnityEngine;
using Tethered.Patterns.StateMachine;
using Tethered.Player.States;
using Tethered.Cameras;
using Tethered.Patterns.ServiceLocator;

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

        public PlayerWeight Weight { get => weight; }

        protected virtual void Awake()
        {
            // Get components
            rb = GetComponent<Rigidbody2D>();
            boxCollider = GetComponent<BoxCollider2D>();
            animator = GetComponentInChildren<Animator>();
            moveableController = GetComponent<MoveableController>();
            skinTransform = animator.transform;

            // Initialize the state machine
            stateMachine = new StateMachine();

            // Create states
            IdleState idleState = new IdleState(this, animator);
            LocomotionState locomotionState = new LocomotionState(this, animator);
            MovingObjectState pushState = new MovingObjectState(this, animator, moveableController);

            // Set up individual states
            SetupStates(idleState, locomotionState);

            // Define state transitions
            stateMachine.At(idleState, locomotionState, new FuncPredicate(() => moveDirectionX != 0));
            stateMachine.At(idleState, pushState, new FuncPredicate(() => moveableController.MovingObject));

            stateMachine.At(locomotionState, idleState, new FuncPredicate(() => moveDirectionX == 0));
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
        protected abstract void SetupStates(IdleState idleState, LocomotionState locomotionState);

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
    }
}