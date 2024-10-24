using UnityEngine;

namespace Tethered.Player.States
{
    public class LocomotionState : PlayerState
    {
        public LocomotionState(PlayerController controller, Animator animator)
            : base(controller, animator)
        {
        }

        public override void OnEnter()
        {
            animator.CrossFade(LocomotionHash, crossFadeDuration);
        }

        public override void FixedUpdate()
        {
            // Move the player
            controller.Move();
        }

        public override void OnExit()
        {
            controller.EndMove();
        }
    }
}