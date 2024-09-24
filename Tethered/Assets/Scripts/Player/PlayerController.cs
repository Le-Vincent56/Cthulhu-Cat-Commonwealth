using UnityEngine;
using Tethered.Patterns.StateMachine;
using Tethered.Player.States;

namespace Tethered.Player
{
    public class PlayerController : MonoBehaviour
    {
        protected Rigidbody2D rb;
        private Animator animator;
        private StateMachine stateMachine;

        protected int moveDirectionX;

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

            // Define state tarnsitions
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
    }
}