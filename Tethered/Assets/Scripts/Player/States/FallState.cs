using UnityEngine;

namespace Tethered.Player.States
{
    public class FallState : PlayerState
    {
        public FallState(PlayerController controller, Animator animator)
            : base(controller, animator)
        { }

        public override void OnEnter()
        {
            animator.CrossFade(FallHash, crossFadeDuration);
        }
    }
}