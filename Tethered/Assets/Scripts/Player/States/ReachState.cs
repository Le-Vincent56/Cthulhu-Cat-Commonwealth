using Tethered.Timers;
using UnityEngine;

namespace Tethered.Player.States
{
    public class ReachState : PlayerState
    {
        private readonly CountdownTimer animationTimer;

        public ReachState(PlayerController controller, Animator animator)
            : base(controller, animator)
        {
            animationTimer = new CountdownTimer(1f);
            animationTimer.OnTimerStop += EndReach;
        }

        public override void OnEnter()
        {
            animator.CrossFade(ReachHash, crossFadeDuration);
            animationTimer.Start();
        }

        /// <summary>
        /// End the player reaching state
        /// </summary>
        private void EndReach()
        {
            controller.SetReaching(false);
        }
    }
}
