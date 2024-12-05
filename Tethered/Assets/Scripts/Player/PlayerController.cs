using UnityEngine;
using Tethered.Patterns.StateMachine;
using Tethered.Player.States;
using Tethered.Cameras;
using Tethered.Patterns.ServiceLocator;
using System.Collections.Generic;
using System.Linq;
using Tethered.Audio;
using static UnityEditor.FilePathAttribute;
using UnityEngine.InputSystem.XR;
using Tethered.Patterns.EventBus;
using Tethered.Monster.Events;

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
        private PlayerSFX playerSFX;

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

        [Header("Pushing")]
        protected bool canPush;

        [Header("Teleport")]
        [SerializeField] private bool teleporting;
        [SerializeField] private Vector3 teleportPosition;

        [Header("Cowering")]
        [SerializeField] private bool cowering;

        [Header("Ground Detection")]
        [SerializeField] private bool grounded;
        [SerializeField] private float castAdjustment;
        [SerializeField] private float castLength;
        [SerializeField] private LayerMask groundLayers;
        [SerializeField] private bool landing;

        private EventBinding<IncreaseAttraction> onIncreaseAttraction;

        public PlayerWeight Weight { get => weight; }
        private Vector2 Up { get; set; }

        protected virtual void Awake()
        {
            // Get components
            rb = GetComponent<Rigidbody2D>();
            boxCollider = GetComponent<BoxCollider2D>();
            animator = GetComponentInChildren<Animator>();
            moveableController = GetComponent<MoveableController>();
            skinTransform = animator.transform;
            playerSFX = GetComponent<PlayerSFX>();

            // Set variables
            initialGravityScale = rb.gravityScale;

            // Initialize the state machine
            stateMachine = new StateMachine();

            // Create states
            IdleState idleState = new IdleState(this, animator);
            LocomotionState locomotionState = new LocomotionState(this, animator, playerSFX);
            ClimbState climbState = new ClimbState(this, animator, playerSFX);
            MovingObjectPrepState moveObjectPrepState = new MovingObjectPrepState(this, animator, moveableController);
            MovingObjectIdleState moveObjectIdleState = new MovingObjectIdleState(this, animator, moveableController);
            MovingObjectLocomotionState moveObjectLocomotionState = new MovingObjectLocomotionState(this, animator, moveableController);
            TeleportState teleportState = new TeleportState(this, animator, skinTransform.GetComponentsInChildren<SpriteRenderer>().ToList());
            FallState fallState = new FallState(this, animator);
            LandState landState = new LandState(this, animator);
            CowerState cowerState = new CowerState(this, animator);

            // Set up individual states
            SetupStates(idleState, locomotionState, climbState);

            // Define state transitions
            stateMachine.At(idleState, locomotionState, new FuncPredicate(() => moveDirectionX != 0));
            stateMachine.At(idleState, climbState, new FuncPredicate(() => climbing));
            stateMachine.At(idleState, moveObjectPrepState, new FuncPredicate(() => moveableController.MovingObject));
            stateMachine.At(idleState, teleportState, new FuncPredicate(() => teleporting));
            stateMachine.At(idleState, cowerState, new FuncPredicate(() => grounded && cowering));
            stateMachine.At(idleState, fallState, new FuncPredicate(() => !grounded && rb.velocity.y < -0.05f));

            stateMachine.At(locomotionState, idleState, new FuncPredicate(() => moveDirectionX == 0));
            stateMachine.At(locomotionState, climbState, new FuncPredicate(() => climbing));
            stateMachine.At(locomotionState, moveObjectPrepState, new FuncPredicate(() => moveableController.MovingObject));
            stateMachine.At(locomotionState, teleportState, new FuncPredicate(() => teleporting));
            stateMachine.At(locomotionState, cowerState, new FuncPredicate(() => grounded && cowering));
            stateMachine.At(locomotionState, fallState, new FuncPredicate(() => !grounded && rb.velocity.y < -0.05f));

            stateMachine.At(climbState, idleState, new FuncPredicate(() => !climbing && moveDirectionX == 0));
            stateMachine.At(climbState, locomotionState, new FuncPredicate(() => !climbing && moveDirectionX != 0));

            stateMachine.At(moveObjectPrepState, moveObjectIdleState, new FuncPredicate(() => moveableController.CanMoveObject && moveDirectionX == 0));
            stateMachine.At(moveObjectPrepState, moveObjectLocomotionState, new FuncPredicate(() => moveableController.CanMoveObject && moveDirectionX != 0));

            stateMachine.At(moveObjectIdleState, moveObjectLocomotionState, new FuncPredicate(() => moveableController.MovingObject && moveDirectionX != 0));
            stateMachine.At(moveObjectIdleState, idleState, new FuncPredicate(() => !moveableController.MovingObject && moveDirectionX == 0));
            stateMachine.At(moveObjectIdleState, locomotionState, new FuncPredicate(() => !moveableController.MovingObject && moveDirectionX != 0));

            stateMachine.At(moveObjectLocomotionState, moveObjectIdleState, new FuncPredicate(() => moveableController.MovingObject && moveDirectionX == 0));
            stateMachine.At(moveObjectLocomotionState, idleState, new FuncPredicate(() => !moveableController.MovingObject && moveDirectionX == 0));
            stateMachine.At(moveObjectLocomotionState, locomotionState, new FuncPredicate(() => !moveableController.MovingObject && moveDirectionX != 0));

            stateMachine.At(teleportState, idleState, new FuncPredicate(() => !teleporting && moveDirectionX == 0));
            stateMachine.At(teleportState, locomotionState, new FuncPredicate(() => !teleporting && moveDirectionX != 0));

            stateMachine.At(fallState, landState, new FuncPredicate(() => grounded));

            stateMachine.At(landState, idleState, new FuncPredicate(() => !landing && moveDirectionX == 0));
            stateMachine.At(landState, locomotionState, new FuncPredicate(() => !landing && moveDirectionX != 0));

            stateMachine.At(cowerState, idleState, new FuncPredicate(() => !cowering && !moveableController.MovingObject && moveDirectionX == 0));
            stateMachine.At(cowerState, locomotionState, new FuncPredicate(() => !cowering && !moveableController.MovingObject && moveDirectionX != 0));

            // Set an initial state
            stateMachine.SetState(idleState);
        }

        private void OnEnable()
        {
            onIncreaseAttraction = new EventBinding<IncreaseAttraction>(StartCowering);
            EventBus<IncreaseAttraction>.Register(onIncreaseAttraction);
        }

        private void OnDisable()
        {
            EventBus<IncreaseAttraction>.Deregister(onIncreaseAttraction);
        }

        protected virtual void Start()
        {
            // Retrieve the Camera boundary and register to it
            cameraBoundary = ServiceLocator.ForSceneOf(this).Get<CameraBoundary>();
            cameraBoundary.Register(this);
        }

        protected virtual void Update()
        {
            CheckGround();

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
        /// Check if the Player is grounded
        /// </summary>
        private void CheckGround()
        {
            // Get the upward direction
            float rotation = rb.rotation * Mathf.Deg2Rad;
            Up = new Vector2(-Mathf.Sin(rotation), Mathf.Cos(rotation));

            // Get the cast position
            Vector2 castPosition = transform.position - new Vector3(0f, castAdjustment, 0f);

            // Raycast downward
            RaycastHit2D groundHit = Physics2D.Raycast(castPosition, -Up, castLength, groundLayers);
            Debug.DrawRay(castPosition, -Up * castLength, Color.red);

            grounded = groundHit;
        }

        /// <summary>
        /// Move the Player
        /// </summary>
        public void Move()
        {
            rb.velocity = new Vector2(moveDirectionX * movementSpeed, rb.velocity.y);
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
            // Disable input
            DisableInput();

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
            // Enable input
            EnableInput();

            climbing = false;
            boxCollider.enabled = true;
            rb.gravityScale = initialGravityScale;
        }

        /// <summary>
        /// Start teleporting
        /// </summary>
        /// <param name="targetPosition"></param>
        public void StartTeleport(Vector3 targetPosition)
        {
            // Set teleporting variables
            teleporting = true;
            teleportPosition = targetPosition;

            // Disable input
            DisableInput();
        }

        /// <summary>
        /// Get the player's teleport position
        /// </summary>
        public Vector3 GetTeleportPosition() => teleportPosition;

        /// <summary>
        /// End teleporting
        /// </summary>
        public void EndTeleport()
        {
            // Unset teleporting variables
            teleporting = false;
            teleportPosition = Vector3.zero;

            // Enable input
            EnableInput();
        }

        /// <summary>
        /// Set whether or not the Player is landing
        /// </summary>
        /// <param name="landing"></param>
        public void SetLanding(bool landing) => this.landing = landing;

        /// <summary>
        /// Start the Player cower
        /// </summary>
        public void StartCowering() => cowering = true;

        /// <summary>
        /// Stop the Player from cowering
        /// </summary>
        public void StopCowering() => cowering = false;
    }
}
