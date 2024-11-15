using Tethered.Audio;
using UnityEngine;

namespace Tethered.Player.States
{
    public class LocomotionState : PlayerState
    {
        private readonly PlayerSFX playerSFX;

        public LocomotionState(PlayerController controller, Animator animator, PlayerSFX playerSFX)
            : base(controller, animator)
        {
            this.playerSFX = playerSFX;
        }

        public override void OnEnter()
        {
            animator.CrossFade(LocomotionHash, crossFadeDuration);

            // Enable footstep sounds
            playerSFX.EnableFootsteps();
        }

        public override void FixedUpdate()
        {
            // Move the player
            controller.Move();
        }

        public override void OnExit()
        {
            controller.EndMove();

            // Disable footstep sounds
            playerSFX.DisableFootsteps();
        }
    }
}