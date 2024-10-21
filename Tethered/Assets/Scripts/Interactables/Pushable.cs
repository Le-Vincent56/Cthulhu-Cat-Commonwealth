using DG.Tweening;
using Tethered.Player;
using UnityEngine;

namespace Tethered.Interactables
{
    public class Pushable : Interactable
    {
        [Header("Tweening Variables")]
        [SerializeField] private float translateDuration;
        private Tween translateTween;

        public override void Interact(InteractController controller)
        {

        }

        /// <summary>
        /// Handle locking into place
        /// </summary>
        public void LockIntoPlace(Vector3 position)
        {
            Translate(position, translateDuration);
        }
        
        /// <summary>
        /// Translate the Pushable
        /// </summary>
        private void Translate(Vector3 position, float duration, TweenCallback onComplete = null)
        {
            // Kill any existing translate tween
            translateTween?.Kill();

            // Translate the transform
            translateTween = transform.DOMove(position, translateDuration);

            // Exit case - no callback function defined
            if (onComplete == null) return;

            // Set callbacks
            translateTween.onComplete += onComplete;
        }
    }
}