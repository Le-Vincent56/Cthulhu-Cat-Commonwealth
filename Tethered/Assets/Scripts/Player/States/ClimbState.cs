using UnityEngine;
using System.Collections;

namespace Tethered.Player.States
{
    public class ClimbState : PlayerState
    {
        public ClimbState(PlayerController controller, Animator animator)
           : base(controller, animator)
        {
        }

        public override void OnEnter()
        {
            animator.CrossFade(ClimbHash, crossFadeDuration);
        }

        public override void Update()
        {
            // Handle climbing
            controller.Climb();
        }

    }
}
