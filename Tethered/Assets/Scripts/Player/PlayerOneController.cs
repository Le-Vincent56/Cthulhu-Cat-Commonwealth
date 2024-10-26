using UnityEngine;
using Tethered.Input;
using Tethered.Player.States;

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
            // TODO: Set up Climb State
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