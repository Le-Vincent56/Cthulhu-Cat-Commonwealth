using DG.Tweening;
using System.Collections.Generic;
using Tethered.Player;
using UnityEngine;

namespace Tethered.Interactables
{
    [RequireComponent(typeof(BoxCollider2D))]
    public class CrawlSpace : MonoBehaviour, IInteractable
    {
        [SerializeField] private SpriteRenderer interactSymbol;
        [SerializeField] private List<Vector2> path;

        private Vector2 initialScale;
        private Vector2 targetScale;

        private Tween fadeTween;
        private Tween scaleTween;
        private float fadeDuration;
        private float scaleDuration;

        private void OnValidate()
        {
            // Set the path
            SetPath();
        }

        private void Awake()
        {
            // Get components
            interactSymbol = GetComponentInChildren<SpriteRenderer>();

            // Set the path
            SetPath();

            // Set scale targets
            initialScale = interactSymbol.transform.localScale * 0.7f;
            targetScale = interactSymbol.transform.localScale;

            // Set animation durations
            fadeDuration = 0.3f;
            scaleDuration = 0.5f;

            // Hide the interact symbol
            interactSymbol.DOFade(0f, 0f);
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            // Exit case - if a PlayerTwoController is not found on the collision object
            if (!collision.TryGetComponent(out PlayerTwoController controller)) return;

            // Set the controller's interactable
            controller.SetInteractable(this);

            // Show the interact symbol
            ShowInteractSymbol();
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            // Exit case - if a PlayerTwoController is not found on the collision object
            if (!collision.TryGetComponent(out PlayerTwoController controller)) return;

            // Set the controller's interactable
            controller.SetInteractable(null);

            // Hide the interact symbol
            HideInteractSymbol();
        }

        /// <summary>
        /// Show the interact symbol
        /// </summary>
        public void ShowInteractSymbol()
        {
            // Fade in
            Fade(1f);

            // Scale to target
            Scale(targetScale);
        }

        /// <summary>
        /// Hide the interact symbol
        /// </summary>
        public void HideInteractSymbol()
        {
            // Fade out
            Fade(0f);

            // Scale to initial
            Scale(initialScale);
        }

        /// <summary>
        /// Go through the Crawl Space
        /// </summary>
        public void Interact(PlayerController controller)
        {
            // Exit case - if the PlayerController is not a PlayerTwoController
            if (controller is not PlayerTwoController youngerController) return;

            // Hide the interact symbol
            HideInteractSymbol();

            // Start crawling for the Younger Sibling
            youngerController.StartCrawl(path);
        }

        /// <summary>
        /// Set the path using child game objects
        /// </summary>
        private void SetPath()
        {
            // Verify that the path list is created
            path ??= new List<Vector2>();

            // Clear the current path
            path.Clear();

            // Add the current position
            path.Add(transform.position);

            // Iterate through each child object and add their position
            // to the path
            for(int i = 1; i < transform.childCount; i++)
            {
                path.Add(transform.GetChild(i).position);
            }
        }

        /// <summary>
        /// Fade using Tweening
        /// </summary>
        private void Fade(float endValue, TweenCallback onComplete = null)
        {
            // Kill the fade tween if it exists
            fadeTween?.Kill();

            // Fade the interact symbol
            fadeTween = interactSymbol.DOFade(endValue, fadeDuration);

            // Exit case - no callback was given
            if (onComplete == null) return;

            // Add completion listeners
            fadeTween.onComplete += onComplete;
        }

        /// <summary>
        /// Scale using Tweening
        /// </summary>
        private void Scale(Vector3 target, TweenCallback onComplete = null)
        {
            // Kill the scale tween if it exists
            scaleTween?.Kill();

            // Scale the interact symbol
            scaleTween = interactSymbol.transform.DOScale(target, scaleDuration)
                    .SetEase(Ease.OutBounce);

            // Exit case - no callback was given
            if (onComplete == null) return;

            // Add completion listeners
            scaleTween.onComplete += onComplete;
        }

        private void OnDrawGizmosSelected()
        {
            // Exit case - if the path is null or doesn't have any objects
            if (path == null || path.Count == 0) return;

            Gizmos.color = Color.green;
            for (int i = 0; i < path.Count - 1; i++)
            {
                Gizmos.DrawLine(path[i], path[i + 1]);
            }
        }
    }
}