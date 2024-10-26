using Tethered.Timers;
using UnityEditor.Animations;
using UnityEngine;

namespace Tethered.Player.States
{
    public class ReachState : PlayerState
    {
        private readonly CountdownTimer animationTimer;

        public ReachState(PlayerController controller, Animator animator)
            : base(controller, animator)
        {
            animationTimer = new CountdownTimer(GetClipLengthFromStateHash(ReachHash));
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
        private void EndReach() => controller.SetReaching(false);

        /// <summary>
        /// Get the clip length of an Animation Clip from its State Hash
        /// </summary>
        private float GetClipLengthFromStateHash(int stateHash)
        {
            // Exit case - if the Animator is not initialized
            if (animator == null) return 0f;

            // Get the AnimatorController
            AnimatorController controller = animator.runtimeAnimatorController as AnimatorController;
            if (controller == null) return 0f;

            // Go through each layer in the controller
            foreach (AnimatorControllerLayer layer in controller.layers)
            {
                // Go through each state in the layer
                foreach (ChildAnimatorState state in layer.stateMachine.states)
                {
                    if (state.state.nameHash == stateHash)
                    {
                        // Get the motion (animation clip) associated with this state
                        AnimationClip clip = state.state.motion as AnimationClip;
                        if (clip != null)
                        {
                            return clip.length;  // Return the length of the animation clip
                        }
                    }
                }
            }

            // Return 0 if no matching state or clip was found
            return 0f;
        }
    }
}
