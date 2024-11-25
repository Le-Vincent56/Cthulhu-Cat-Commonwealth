using System.Collections.Generic;
using Tethered.Interactables.Events;
using Tethered.Patterns.EventBus;
using Tethered.Player;
using UnityEngine;

namespace Tethered.Interactables
{
    public class CrawlSpace : Interactable
    {
        [SerializeField] private int hash;
        [SerializeField] private bool oneTimeUse;
        [SerializeField] private List<Vector2> path;
        private BoxCollider2D boxCollider;

        private EventBinding<DisableInteractables> disableInteractablesEvent;

        private void OnValidate()
        {
            // Set the path
            SetPath();
        }

        protected override void Awake()
        {
            base.Awake();

            boxCollider = GetComponent<BoxCollider2D>();

            // Set the path
            SetPath();
        }

        private void OnEnable()
        {
            disableInteractablesEvent = new EventBinding<DisableInteractables>(Disable);
            EventBus<DisableInteractables>.Register(disableInteractablesEvent);
        }

        private void OnDisable()
        {
            EventBus<DisableInteractables>.Deregister(disableInteractablesEvent);
        }

        protected override void OnTriggerEnter2D(Collider2D collision)
        {
            // Exit case - if a PlayerTwoController is not found on the collision object
            if (!collision.TryGetComponent(out InteractController controller)) return;

            // Exit case - if not the Younger Sibling
            if (controller.PlayerType != PlayerType.Younger) return;

            // Set the controller's interactable
            controller.SetInteractable(this);

            // Add the controller to the hashset
            controllers.Add(controller);

            // Get the entering PlayerController
            PlayerController enteringPlayer = controller.GetComponent<PlayerController>();

            // Decide the sprite based on the entering player
            DecideEnterSprite(enteringPlayer);

            if (playerTypeRestricted)
            {
                if (enteringPlayer is PlayerOneController && allowedType != PlayerType.Older) return;
                if (enteringPlayer is PlayerTwoController && allowedType != PlayerType.Younger) return;
            }

            // Show the interact symbol
            ShowInteractSymbol();
        }

        protected override void OnTriggerExit2D(Collider2D collision)
        {
            // Exit case - if a PlayerTwoController is not found on the collision object
            if (!collision.TryGetComponent(out InteractController controller)) return;

            // Exit case - if not the Younger Sibling
            if (controller.PlayerType != PlayerType.Younger) return;

            // Set the controller's interactable
            controller.SetInteractable(null);

            // Remove the controller from the hashset
            controllers.Remove(controller);

            // Decide the sprite based on the remaining players in range
            DecideExitSprite();

            // Hide the interact symbol
            HideInteractSymbol();
        }

        /// <summary>
        /// Go through the Crawl Space
        /// </summary>
        public override void Interact(InteractController controller)
        {
            // Exit case - if there is no PlayerTwoController attached to the controller
            if (!controller.TryGetComponent(out PlayerTwoController playerController)) return;

            // Hide the interact symbol
            HideInteractSymbol();

            // Start crawling for the Younger Sibling
            playerController.StartCrawl(path);

            // Exit case - if not a one-time use
            if (!oneTimeUse) return;

            // End the interaction
            OnEnd();
        }

        /// <summary>
        /// Disable the Crawl Space
        /// </summary>
        protected override void OnEnd() => Disable(new DisableInteractables() { Hash = hash });

        /// <summary>
        /// Callback function to disable the Crawl Sapce
        /// </summary>
        protected void Disable(DisableInteractables eventData)
        {
            // Exit case - Hash mis-match
            if (eventData.Hash != hash) return;

            // Deactivate the gameObject
            boxCollider.enabled = false;
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