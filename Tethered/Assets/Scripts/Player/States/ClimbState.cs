using UnityEngine;
using Tethered.Audio;

namespace Tethered.Player.States
{
    public class ClimbState : PlayerState
    {
        private readonly PlayerSFX playerSFX;

        public ClimbState(PlayerController controller, Animator animator, PlayerSFX playerSFX)
           : base(controller, animator)
        {
            this.playerSFX = playerSFX;
        }

        public override void OnEnter()
        {
            animator.CrossFade(ClimbHash, crossFadeDuration);

            // Start playing SFX
            playerSFX.EnableLadder();
        }

        public override void Update()
        {
            // Handle climbing
            controller.Climb();
        }

        public override void OnExit()
        {
            // Stop playing SFX
            playerSFX.DisableLadder();
        }
    }
}
