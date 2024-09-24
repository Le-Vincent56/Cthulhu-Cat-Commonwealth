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
    }
}