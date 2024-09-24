using UnityEngine;

namespace Tethered.Player.States
{
    public class IdleState : PlayerState
    {
        public IdleState(PlayerController controller, Animator animator) 
            : base(controller, animator)
        {
        }

        public override void OnEnter()
        {
            animator.CrossFade(IdleHash, crossFadeDuration);
        }
    }
}