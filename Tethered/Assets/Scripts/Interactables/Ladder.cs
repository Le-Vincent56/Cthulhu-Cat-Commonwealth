using System.Collections.Generic;
using Tethered.Interactables.Events;
using Tethered.Patterns.EventBus;
using Tethered.Player;
using UnityEngine;

namespace Tethered.Interactables
{
    public class Ladder : Interactable
    {
        [SerializeField] private int hash;
        [SerializeField] private bool mustExtend;
        [SerializeField] private bool climable;

        [SerializeField] private List<Vector2> path;

        private EventBinding<EnableLadder> onEnableLadder;

        private void OnEnable()
        {
            onEnableLadder = new EventBinding<EnableLadder>(EnableLadder);
            EventBus<EnableLadder>.Register(onEnableLadder);
        }

        private void OnDisable()
        {
            EventBus<EnableLadder>.Deregister(onEnableLadder);
        }

        private void OnValidate()
        {
            // Set the path
            SetPath();
        }

        protected override void Awake()
        {
            base.Awake();

            // If the ladder doesn't need to be extended, set it to climable
            if (!mustExtend) climable = true;

            // Set the path
            SetPath();
        }

        protected override void OnTriggerEnter2D(Collider2D collision)
        {
            // Exit case - the ladder is not climable
            if (!climable) return;

            // Exit case - if a PlayerController is not found on the collision object
            if (!collision.TryGetComponent(out InteractController controller)) return;

            // Set the controller's interactable
            controller.SetInteractable(this);

            // Add the controller to the hashset
            controllers.Add(controller);

            // Get the entering PlayerController
            PlayerController enteringPlayer = controller.GetComponent<PlayerController>();

            // Decide the sprite based on the entering player
            DecideEnterSprite(enteringPlayer);

            // Show the interact symbol
            ShowInteractSymbol();
        }

        protected override void OnTriggerExit2D(Collider2D collision)
        {
            // Exit case - the ladder is not climable
            if (!climable) return;

            // Exit case - if a PlayerController is not found on the collision object
            if (!collision.TryGetComponent(out InteractController controller)) return;

            // Set the controller's interactable
            controller.SetInteractable(null);

            // Remove the controller from the hashset
            controllers.Remove(controller);

            // Decide the sprite based on the remaining players in range
            DecideExitSprite();

            // Hide the interact symbol
            if(controllers.Count <= 0)
                HideInteractSymbol();
        }

        /// <summary>
        /// Go up the Laddder
        /// </summary>
        public override void Interact(InteractController controller)
        {
            // Exit case - the ladder is not climable
            if (!climable) return;

            // Exit case - if there is no PlayerController attached to the controller
            if (!controller.TryGetComponent(out PlayerController playerController)) return;

            // Hide the interact symbol
            if (controllers.Count <= 0)
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

        /// <summary>
        /// Enable the ladder to be climable
        /// </summary>
        private void EnableLadder(EnableLadder eventData)
        {
            // Exit case - if the hash is incorrect
            if (hash != eventData.Hash) return;

            // Set the ladder to climable
            climable = true;
        }

        /// <summary>
        /// Set the Ladder's hash
        /// </summary>
        public void SetHash(int hash) => this.hash = hash;

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

