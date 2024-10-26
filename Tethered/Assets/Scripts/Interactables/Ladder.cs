using System.Collections.Generic;
using Tethered.Player;
using UnityEngine;

namespace Tethered.Interactables
{
    public class Ladder : Interactable
    {
        [SerializeField] private List<Vector2> path;

        private void OnValidate()
        {
            // Set the path
            SetPath();
        }

        protected override void Awake()
        {
            base.Awake();

            // Set the path
            SetPath();
        }

        protected override void OnTriggerEnter2D(Collider2D collision)
        {
            // Exit case - if a PlayerController is not found on the collision object
            if (!collision.TryGetComponent(out InteractController controller)) return;

            // Set the controller's interactable
            controller.SetInteractable(this);

            // Show the interact symbol
            ShowInteractSymbol();
        }

        protected override void OnTriggerExit2D(Collider2D collision)
        {
            // Exit case - if a PlayerController is not found on the collision object
            if (!collision.TryGetComponent(out InteractController controller)) return;

            // Set the controller's interactable
            controller.SetInteractable(null);

            // Hide the interact symbol
            HideInteractSymbol();
        }

        /// <summary>
        /// Go up the Laddder
        /// </summary>
        public override void Interact(InteractController controller)
        {
            // Exit case - if there is no PlayerController attached to the controller
            if (!controller.TryGetComponent(out PlayerController playerController)) return;

            // Hide the interact symbol
            HideInteractSymbol();

            // Start climbing
            playerController.StartClimb(path);
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
            for (int i = 1; i < transform.childCount; i++)
            {
                path.Add(transform.GetChild(i).position);
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

