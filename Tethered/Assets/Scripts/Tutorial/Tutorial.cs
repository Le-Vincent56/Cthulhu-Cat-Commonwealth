using DG.Tweening;
using UnityEngine;

namespace Tethered.Tutorial
{
    public class TutorialManager : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private SpriteRenderer leftTutorial;
        [SerializeField] private SpriteRenderer rightTutorial;        
        [SerializeField] private SpriteRenderer interactTutorial;

        [Header("Variables")]
        [SerializeField] private bool hasMovedLeft;
        [SerializeField] private bool hasMovedRight;
        [SerializeField] private bool hasInteracted;
        [SerializeField] private float usedOpacity;

        [Header("Tweening Variables")]
        [SerializeField] private float fadeDuration;
        private Tween fadeLeftTween;
        private Tween fadeRightTween;
        private Tween fadeInteractTween;

        private void Awake()
        {
            // Get components
            SpriteRenderer[] spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
            leftTutorial = spriteRenderers[0];
            rightTutorial = spriteRenderers[1];
            interactTutorial = spriteRenderers[2];

            // Set variables
            hasMovedLeft = false;
            hasMovedRight = false;
            hasInteracted = false;
        }

        private void Start()
        {
            // Show the Movement Tutorial immediately
            ShowMovementTutorial();
        }

        /// <summary>
        /// Show the Movement tutorial
        /// </summary>
        private void ShowMovementTutorial()
        {
            // Fade in both Left and Right Movement tutorials
            FadeMovedLeft(1f, fadeDuration);
            FadeMovedRight(1f, fadeDuration);
        }

        /// <summary>
        /// Show the Interact tutorial
        /// </summary>
        public void ShowInteractTutorial()
        {
            // Exit case - has already activated the Interact tutorial
            if (hasInteracted) return;

            // Fade in the Interact tutorial
            FadeInteract(1f, fadeDuration);
        }

        /// <summary>
        /// Hide the Interact tutorial
        /// </summary>
        public void HideInteractTutorial()
        {
            // Exit case - has already activated the Interact tutorial
            if (hasInteracted) return;

            // Fade in the Interact tutorial
            FadeInteract(0f, fadeDuration);
        }

        /// <summary>
        /// Use the Left Movement tutorial
        /// </summary>
        public void UseLeft()
        {
            // Exit case - tutorial has already been activated
            if (hasMovedLeft) return;

            // Set to moved left
            hasMovedLeft = true;

            // Exit case - if removing movement tutorial
            if (CheckRemoveMovement()) return;

            // Fade the left tutorial to used
            FadeMovedLeft(usedOpacity, fadeDuration);
        }

        /// <summary>
        /// Use the Right Movement tutorial
        /// </summary>
        public void UseRight()
        {
            // Exit case - tutorial has already been activated
            if (hasMovedRight) return;

            // Set to moved right
            hasMovedRight = true;

            // Exit case - if removing movement tutorial
            if (CheckRemoveMovement()) return;

            // Fade the right tutorial to used
            FadeMovedRight(usedOpacity, fadeDuration);
        }

        /// <summary>
        /// Use the Interact tutorial
        /// </summary>
        public void UseInteract()
        {
            // Exit case - tutorial has already been activated
            if (hasInteracted) return;

            // Set to interacted
            hasInteracted = true;

            // Fade the interact tutorial out
            FadeInteract(0f, fadeDuration);
        }

        /// <summary>
        /// Check if the movement tutorial should be removed
        /// </summary>
        private bool CheckRemoveMovement()
        {
            // Exit case - both movements have not been used
            if (!hasMovedRight || !hasMovedLeft) return false;

            FadeMovedLeft(0f, fadeDuration);
            FadeMovedRight(0f, fadeDuration);

            return true;
        }

        /// <summary>
        /// Handle fade-tweening the left movement tutorial
        /// </summary>
        private void FadeMovedLeft(float endValue, float duration)
        {
            // Kill the Fade Left Tween if it exists
            fadeLeftTween?.Kill();

            // Set the Fade Left Tween
            fadeLeftTween = leftTutorial.DOFade(endValue, duration);
        }

        /// <summary>
        /// Handle fade-tweening the right movement tutorial
        /// </summary>
        private void FadeMovedRight(float endValue, float duration)
        {
            // Kill the Fade Right Tween if it exists
            fadeRightTween?.Kill();

            // Set the Fade Right Tween
            fadeRightTween = rightTutorial.DOFade(endValue, duration);
        }

        /// <summary>
        /// Handle fade-tweening the interact tutorial
        /// </summary>
        private void FadeInteract(float endValue, float duration)
        {
            // Kill the Fade Interact Tween if it exists
            fadeInteractTween?.Kill();

            // Set the Fade Interact Tween
            fadeInteractTween = interactTutorial.DOFade(endValue, duration);
        }
    }
}