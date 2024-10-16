using System.Collections.Generic;
using Tethered.Player;
using UnityEngine;

namespace Tethered.Interactables
{
    [RequireComponent(typeof(BoxCollider2D))]
    public class CrawlSpace : MonoBehaviour, IInteractable
    {
        [SerializeField] private List<Vector2> path;

        private void OnValidate()
        {
            // Set the path
            SetPath();
        }

        private void Awake()
        {
            // Set the path
            SetPath();
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            // Exit case - if a PlayerTwoController is not found on the collision object
            if (!collision.TryGetComponent(out PlayerTwoController controller)) return;

            // Set the controller's interactable
            controller.SetInteractable(this);
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            // Exit case - if a PlayerTwoController is not found on the collision object
            if (!collision.TryGetComponent(out PlayerTwoController controller)) return;

            // Set the controller's interactable
            controller.SetInteractable(null);
        }

        /// <summary>
        /// Go through the Crawl Space
        /// </summary>
        public void Interact(PlayerController controller)
        {
            // Exit case - if the PlayerController is not a PlayerTwoController
            if (controller is not PlayerTwoController youngerController) return;

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
            foreach(Transform child in transform)
            {
                path.Add(child.position);
            }
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