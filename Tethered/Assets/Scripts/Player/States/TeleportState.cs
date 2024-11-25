using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

namespace Tethered.Player.States
{
    public class TeleportState : PlayerState
    {
        private Sequence fadeSequence;
        private Tween translateTween;
        private List<SpriteRenderer> spriteRenderers;
        private readonly float fadeDuration;
        private readonly float translateDuration;
        private Vector3 targetPosition;

        public TeleportState(PlayerController controller, Animator animator, List<SpriteRenderer> spriteRenderers)
            : base(controller, animator)
        {
            // Set variables
            this.spriteRenderers = spriteRenderers;
            fadeDuration = 0.5f;
            translateDuration = 1.5f;
            targetPosition = Vector3.zero;
        }

        public override void OnEnter()
        {
            // Fade out
            Fade(0f, fadeDuration, () =>
            {
                // Teleport the player
                targetPosition = controller.GetTeleportPosition();

                // Move the player down to the target position
                Translate(targetPosition, translateDuration, () =>
                {
                    // Fade back in and then end the teleport
                    Fade(1f, fadeDuration, () => controller.EndTeleport());
                });
            });
        }

        public override void OnExit()
        {
            // Kill the fade Sequence
            fadeSequence?.Kill();

            // Reset target position
            targetPosition = Vector3.zero;
        }

        private void Translate(Vector3 endValue, float duration, TweenCallback onComplete = null)
        {
            // Kill the current translate tween if it exists
            translateTween?.Kill();

            // Translate the controller transform to the target position
            translateTween = controller.transform.DOMove(targetPosition, duration).SetEase(Ease.InOutQuad);

            // Exit case - no completion action was given
            if (onComplete == null) return;

            // Hook up completion actions
            translateTween.onComplete += onComplete;
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

            // Exit case - no completion action was given
            if (onComplete == null) return;

            // Hook up completion actions
            fadeSequence.onComplete += onComplete;
        }
    }
}
