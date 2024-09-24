using UnityEngine;
using Tethered.Input;

namespace Tethered.Player
{
    public class PlayerTwoController : PlayerController
    {
        [Header("Input")]
        [SerializeField] private PlayerTwoInputReader inputReader;

        [Header("Movement")]
        [SerializeField] private float movementSpeed;

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
    }
}