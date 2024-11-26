using Tethered.Timers;
using UnityEngine;

namespace Tethered.Player.States
{
    public class LandState : PlayerState
    {
        private readonly CountdownTimer landTimer;

        public LandState(PlayerController controller, Animator animator)
            : base(controller, animator)
        {
            // Create the Countdown Timer
            landTimer = new CountdownTimer(0.5f);

            // Hook up events to unset landing when the Countdown Timer has ended
            landTimer.OnTimerStop += () => controller.SetLanding(false);
        }

        public override void OnEnter()
        {
            // Set the animation
            animator.CrossFade(LandHash, crossFadeDuration);

            // Disable controller input
            controller.DisableInput();

            // Start the land timer
            landTimer.Reset();
            landTimer.Start();

            // Set the player as landing
            controller.SetLanding(true);
        }

        public override void OnExit()
        {
            // Enable player input
            controller.EnableInput();
        }
    }
}
