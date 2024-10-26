using UnityEngine;
using Tethered.Patterns.StateMachine;
using Tethered.Player.States;
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
        protected Animator animator;
        protected BoxCollider2D boxCollider;
        protected StateMachine stateMachine;

        protected int moveDirectionX;
        [SerializeField] protected PlayerWeight weight;

        [Header("Climbing")]
        [SerializeField] protected bool climbing;
        [SerializeField] private int currentLadderPathIndex;
        [SerializeField] private List<Vector2> ladderPath;

        public PlayerWeight Weight { get => weight; }

        protected virtual void Awake()
        {
            // Get components
            rb = GetComponent<Rigidbody2D>(); 
            animator = GetComponentInChildren<Animator>();
            boxCollider = GetComponent<BoxCollider2D>();

            // Initialize the state machine
            stateMachine = new StateMachine();

            // Create states
            IdleState idleState = new IdleState(this, animator);
            LocomotionState locomotionState = new LocomotionState(this, animator);
            ClimbState climbState = new ClimbState(this, animator);


            // Set up individual states
            SetupStates(idleState, locomotionState, climbState);

            // Define state transitions
            stateMachine.At(idleState, locomotionState, new FuncPredicate(() => moveDirectionX != 0));
            stateMachine.At(locomotionState, idleState, new FuncPredicate(() => moveDirectionX == 0));
            stateMachine.At(idleState, climbState, new FuncPredicate(() => climbing));
            stateMachine.At(locomotionState, climbState, new FuncPredicate(() => climbing));
            stateMachine.At(climbState, idleState, new FuncPredicate(() => !climbing && moveDirectionX == 0));
            stateMachine.At(climbState, locomotionState, new FuncPredicate(() => !climbing && moveDirectionX != 0));

            // Set an initial state
            stateMachine.SetState(idleState);
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
        /// Start climbing
        /// </summary>
        public void StartClimb(List<Vector2> path)
        {
            // Set crawling to true
            climbing = true;

            Debug.Log("I'm climbing!!");

            // Initialize the path
            this.ladderPath = path;
            currentLadderPathIndex = 1;
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
        }
    }
}
