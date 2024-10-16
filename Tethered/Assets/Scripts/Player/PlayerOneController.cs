using UnityEngine;
using Tethered.Input;
using Tethered.Player.States;

namespace Tethered.Player
{
    public class PlayerOneController : PlayerController
    {
        [Header("Input")]
        [SerializeField] private PlayerOneInputReader inputReader;

        [Header("Movement")]
        [SerializeField] private float movementSpeed;

        private void OnEnable()
        {
            inputReader.Interact += Interact;
        }

        private void OnDisable()
        {
            inputReader.Interact -= Interact;
        }

        protected override void Update()
        {
            // Update movement direction
            moveDirectionX = inputReader.NormMoveX;

            // Update the state machine
            base.Update();
        }

        protected override void FixedUpdate()
        {
            // Set the player velocity
            rb.velocity = new Vector2(moveDirectionX * movementSpeed, 0);

            // Update the state machine
            base.FixedUpdate();
        }

        /// <summary>
        /// Set up the Older Sibling's individual states
        /// </summary>
        protected override void SetupStates(IdleState idleState, LocomotionState locomotionState)
        {
            // TODO: Set up Climb State
        }
    }
}