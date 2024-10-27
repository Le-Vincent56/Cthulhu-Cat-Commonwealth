using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

namespace Tethered.Player.States
{
    public class TeleportState : PlayerState
    {
        private Sequence fadeSequence;
        private List<SpriteRenderer> spriteRenderers;
        private float fadeDuration;

        public TeleportState(PlayerController controller, Animator animator, List<SpriteRenderer> spriteRenderers)
            : base(controller, animator)
        {
            // Set variables
            this.spriteRenderers = spriteRenderers;
            fadeDuration = 0.5f;
        }

        public override void OnEnter()
        {
            // Fade out
            Fade(0f, fadeDuration, () =>
            {
                // Teleport the player
                controller.TeleportToTargetPosition();

                // Fade back in and then end the teleport
                Fade(1f, fadeDuration, () => controller.EndTeleport());
            });
        }

        public override void OnExit()
        {
            // Kill the fade Sequence
            fadeSequence?.Kill();
        }

        /// <summary>
        /// Handle sprite fading
        /// </summary>
        private void Fade(float endValue, float duration, TweenCallback onComplete = null)
        {
            // Kill the current fade Sequence
            fadeSequence?.Kill();

            // Instantiate the new Sequence
            fadeSequence = DOTween.Sequence();

            // Iterate through each Sprite Renderer
            foreach(SpriteRenderer spriteRenderer in spriteRenderers)
            {
                // Join their fade Tweens
                fadeSequence.Join(spriteRenderer.DOFade(endValue, duration));
            }

            // Exit case - no completin action was given
            if (onComplete == null) return;

            // Hook up completion actions
            fadeSequence.onComplete += onComplete;
        }
    }
}
