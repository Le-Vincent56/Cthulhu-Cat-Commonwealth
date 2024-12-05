using UnityEngine;

namespace Tethered.Player.States
{
    public class MovingObjectLocomotionState : PlayerState
    {
        private readonly MoveableController moveableController;

        public MovingObjectLocomotionState(
            PlayerController controller, 
            Animator animator, 
            MoveableController moveableController
        ) : base(controller, animator)
        {
            this.moveableController = moveableController;
        }

        public override void OnEnter()
        {
            // Allow the Player to move objects
            moveableController.CanMoveObject = true;

            // Cross fade into the animation
            animator.CrossFade(MoveObjectLocomotion, crossFadeDuration);
        }

        public override void FixedUpdate()
        {
            // Exit case - if the Player can;t move objects
            if (!moveableController.CanMoveObject) return;

            // Move the object
            moveableController.MoveObject();
        }

        public override void OnExit()
        {
            // Don't allow the Player to move objects
            moveableController.CanMoveObject = false;
        }
    }
}
