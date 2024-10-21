using UnityEngine;

namespace Tethered.Player.States
{
    public class MovingObjectState : PlayerState
    {
        private readonly MoveableController moveableController;
        private bool canMoveObjects;

        public MovingObjectState(
            PlayerController controller, 
            Animator animator, 
            MoveableController moveableController
        ) : base(controller, animator)
        {
            this.moveableController = moveableController;
        }

        public override void OnEnter()
        {
            // Disable input to prevent movement
            controller.DisableInput();

            // Set the position of the Player
            moveableController.PositionWithMoveable(() =>
            {
                // Cross fade into the animation
                animator.CrossFade(MoveObjectHash, crossFadeDuration);

                // Allow the Player to move objects
                canMoveObjects = true;

                // Enable input
                controller.EnableInput();
            });
        }

        public override void FixedUpdate()
        {
            // Exit case - if the Player can;t move objects
            if (!canMoveObjects) return;

            // Move the object
            moveableController.MoveObject();
        }

        public override void OnExit()
        {
            // Don't allow the Player to move objects
            canMoveObjects = false;
        }
    }
}
