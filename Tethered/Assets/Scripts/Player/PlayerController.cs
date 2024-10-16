using UnityEngine;
using Tethered.Patterns.StateMachine;
using Tethered.Player.States;

namespace Tethered.Player
{
    public enum PlayerWeight
    {
        Light = 0, 
        Heavy = 1
    }

    public abstract class PlayerController : MonoBehaviour
    {
        protected Rigidbody2D rb;
        private Animator animator;
        private StateMachine stateMachine;

        protected int moveDirectionX;
        [SerializeField] protected PlayerWeight weight;

        public PlayerWeight Weight { get => weight; }

        protected virtual void Awake()
        {
            // Get components
            rb = GetComponent<Rigidbody2D>(); 
            animator = GetComponentInChildren<Animator>();

            // Initialize the state machine
            stateMachine = new StateMachine();

            // Create states
            IdleState idleState = new IdleState(this, animator);
            LocomotionState locomotionState = new LocomotionState(this, animator);

            // Set up individual states
            SetupStates();

            // Define state transitions
            stateMachine.At(idleState, locomotionState, new FuncPredicate(() => moveDirectionX != 0));
            stateMachine.At(locomotionState, idleState, new FuncPredicate(() => moveDirectionX == 0));

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
        protected abstract void SetupStates();
    }
}