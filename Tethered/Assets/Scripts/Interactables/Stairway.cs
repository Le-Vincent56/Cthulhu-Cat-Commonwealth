using System;
using Tethered.Interactables.Events;
using Tethered.Patterns.EventBus;
using Tethered.Player;
using UnityEngine;
using UnityEngine.InputSystem.XR;

namespace Tethered.Interactables
{
    public class Stairway : Interactable
    {
        [SerializeField] private int hash;
        [SerializeField] private int toHash;
        [SerializeField] private Transform teleportPosition;

        private EventBinding<ActivateStairs> onActivateStairs;

        private void OnEnable()
        {
            onActivateStairs = new EventBinding<ActivateStairs>(StartTeleport);
            EventBus<ActivateStairs>.Register(onActivateStairs);
        }

        private void OnDisable()
        {
            EventBus<ActivateStairs>.Deregister(onActivateStairs);
        }

        protected override void OnTriggerExit2D(Collider2D collision)
        {
            // Exit case - if a PlayerTwoController is not found on the collision object
            if (!collision.TryGetComponent(out InteractController controller)) return;

            ExitStairway(controller);
        }

        /// <summary>
        /// Start the player teleport
        /// </summary>
        /// <param name="eventData"></param>
        private void StartTeleport(ActivateStairs eventData)
        {
            // Exit case - if the hashes do not match
            if (hash != eventData.Hash) return;

            // Start the player teleport to this position
            eventData.PlayerController.StartTeleport(teleportPosition.position);
        }

        public override void Interact(InteractController controller)
        {
            // Exit the stairway
            ExitStairway(controller);

            // Activate the stairs
            EventBus<ActivateStairs>.Raise(new ActivateStairs()
            {
                Hash = toHash,
                PlayerController = controller.GetComponent<PlayerController>()
            });
        }

        /// <summary>
        /// Exit the stairway
        /// </summary>
        private void ExitStairway(InteractController controller)
        {
            // Set the controller's interactable
            controller.SetInteractable(null);

            // Remove the controller from the hashset
            controllers.Remove(controller);

            // Decide the sprite based on the remaining players in range
            DecideExitSprite();

            // Hide the interact symbol if there are no present controllers
            if (controllers.Count <= 0)
                HideInteractSymbol();
        }

        /// <summary>
        /// Set the Stairway's hash
        /// </summary>
        public void SetHash(int hash) => this.hash = hash;
    }
}