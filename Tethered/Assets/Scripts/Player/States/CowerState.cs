using Tethered.Timers;
using UnityEngine;

namespace Tethered.Player.States
{
    public class CowerState : PlayerState
    {
        private readonly CountdownTimer cowerTimer;

        public CowerState(PlayerController controller, Animator animator) 
            : base(controller, animator)
        {
            // Initialize the Countdown Timer
            cowerTimer = new CountdownTimer(1f);

            // Hook up events to stop cowering when the Countdown Timer ends
            cowerTimer.OnTimerStop += () => controller.StopCowering();
        }

        public override void OnEnter()
        {
            // Disable controller input
            controller.DisableInput();

            // Start the cower Timer
            cowerTimer.Reset();
            cowerTimer.Start();

            // Start the Cower animation
            animator.CrossFade(CowerHash, crossFadeDuration);
        }

        public override void OnExit()
        {
            // Enable controller input
            controller.EnableInput();
        }
    }
}
