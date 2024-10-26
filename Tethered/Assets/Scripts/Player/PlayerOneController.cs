using UnityEngine;
using Tethered.Input;
using Tethered.Player.States;
using Tethered.Patterns.StateMachine;

namespace Tethered.Player
{
    public class PlayerOneController : PlayerController
    {
        [Header("Input")]
        [SerializeField] private PlayerOneInputReader inputReader;

        protected override void Update()
        {
            // Update movement direction
            moveDirectionX = inputReader.NormMoveX;

            // Update the state machine
            base.Update();
        }

        /// <summary>
        /// Set up the Older Sibling's individual states
        /// </summary>
        protected override void SetupStates(IdleState idleState, LocomotionState locomotionState, ClimbState climbState)
        {
            // Create states
            ReachState reachState = new ReachState(this, animator);

            // Define state transitions
            stateMachine.At(idleState, reachState, new FuncPredicate(() => reaching));
            stateMachine.At(locomotionState, reachState, new FuncPredicate(() => reaching));
            stateMachine.At(climbState, reachState, new FuncPredicate(() => reaching));

            stateMachine.At(reachState, idleState, new FuncPredicate(() => !reaching && moveDirectionX == 0));
            stateMachine.At(reachState, locomotionState, new FuncPredicate(() => !reaching && moveDirectionX != 0));
            stateMachine.At(reachState, climbState, new FuncPredicate(() => climbing));
        }

        /// <summary>
        /// Enable Player One's input
        /// </summary>
        public override void EnableInput() => inputReader.Enable();

        /// <summary>
        /// Disable Player One's input
        /// </summary>
        public override void DisableInput() => inputReader.Disable();
    }
}