using UnityEngine;
using Tethered.Patterns.StateMachine;
using Tethered.Player.States;
using Tethered.Interactables;

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
        protected Animator animator;
        protected BoxCollider2D boxCollider;
        protected StateMachine stateMachine;

        protected int moveDirectionX;
        [SerializeField] protected PlayerWeight weight;

        protected IInteractable currentInteractable;

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

            // Set up individual states
            SetupStates(idleState, locomotionState);

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
        protected abstract void SetupStates(IdleState idleState, LocomotionState locomotionState);

        /// <summary>
        /// Set the current Interactable for the Player
        /// </summary>
        public void SetInteractable(IInteractable interactable) => currentInteractable = interactable;

        /// <summary>
        /// Interact with the current interactable
        /// </summary>
        protected void Interact() => currentInteractable.Interact(this);
    }
}