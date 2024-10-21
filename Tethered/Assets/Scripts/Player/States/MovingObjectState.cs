using UnityEngine;

namespace Tethered.Player.States
{
    public class MovingObjectState : PlayerState
    {
        private readonly MoveableController moveableController;
        private bool canMove;

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
            // Set the position of the player
            moveableController.PositionWithMoveable(() =>
            {
                // Cross fade into the animation
                animator.CrossFade(MoveObjectHash, crossFadeDuration);

                // Allow the player to move
                canMove = true;
            });
        }

        public override void FixedUpdate()
        {
            // Exit case - if can't move
            if (!canMove) return;

            // Move the object
            moveableController.MoveObject();
        }
    }
}
