using System.Collections.Generic;
using Tethered.Input;
using Tethered.Interactables;
using Tethered.Inventory;
using UnityEngine;

namespace Tethered.Player
{
    public class InteractController : MonoBehaviour
    {
        [SerializeField] private PlayerType playerType;
        [SerializeField] private PlayerOneInputReader playerOneInputReader;
        [SerializeField] private PlayerTwoInputReader playerTwoInputReader;
        private PlayerInventory inventory;
        [SerializeField] private Interactable currentInteractable;

        // climb
        [SerializeField] private bool climbing;
        [SerializeField] private int currentPathIndex;
        [SerializeField] private List<Vector2> path;

        public PlayerType PlayerType { get => playerType; }
        public PlayerInventory Inventory { get => inventory; }

        private void Awake()
        {
            // Get components
            inventory = GetComponent<PlayerInventory>();
        }

        private void OnEnable()
        {
            switch (playerType)
            {
                case PlayerType.Older:
                    playerOneInputReader.Interact += Interact;
                    break;

                case PlayerType.Younger:
                    playerTwoInputReader.Interact += Interact;
                    break;
            }
        }

        private void OnDisable()
        {
            switch (playerType)
            {
                case PlayerType.Older:
                    playerOneInputReader.Interact -= Interact;
                    break;

                case PlayerType.Younger:
                    playerTwoInputReader.Interact -= Interact;
                    break;
            }
        }

        /// <summary>
        /// Set the current Interactable for the Player
        /// </summary>
        public void SetInteractable(Interactable interactable)
        {
            // Exit case - there is no Player Controller component
            if (!TryGetComponent(out MoveableController moveableController)) return;

            // Exit case - if moving an object
            if (moveableController.MovingObject) return;

            // Set the interactable
            currentInteractable = interactable;
        }

        /// <summary>
        /// Get whether or not the Player currently has the Interactable
        /// </summary>
        public bool HasInteractable(Interactable interactableToCompare) => currentInteractable = interactableToCompare;

        /// <summary>
        /// Interact with the current interactable
        /// </summary>
        protected void Interact() => currentInteractable?.Interact(this);
    }
}