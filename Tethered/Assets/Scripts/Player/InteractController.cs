using System.Collections.Generic;
using Tethered.Input;
using Tethered.Interactables;
using UnityEngine;

namespace Tethered.Player
{
    public class InteractController : MonoBehaviour
    {
        [SerializeField] private PlayerType playerType;
        [SerializeField] private PlayerOneInputReader playerOneInputReader;
        [SerializeField] private PlayerTwoInputReader playerTwoInputReader;
        [SerializeField] protected Interactable currentInteractable;
        protected HashSet<int> keys;

        public PlayerType PlayerType { get => playerType; }
        public HashSet<int> Keys { get => keys; }

        private void Awake()
        {
            // Initialize the hashset
            keys = new HashSet<int>();
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
        public void SetInteractable(Interactable interactable) => currentInteractable = interactable;

        /// <summary>
        /// Get whether or not the Player currently has the Interactable
        /// </summary>
        public bool HasInteractable(Interactable interactableToCompare) => currentInteractable = interactableToCompare;

        /// <summary>
        /// Interact with the current interactable
        /// </summary>
        protected void Interact() => currentInteractable?.Interact(this);

        /// <summary>
        /// Add a Key to the Player's key set
        /// </summary>
        public void AddKey(int hash) => keys.Add(hash);

        /// <summary>
        /// Check a Key in the Player's key set
        /// </summary>
        public void CheckKey(Door door)
        {
            // Exit case - if the key is not found
            if (!keys.Contains(door.Hash)) return;

            // Unlock the door
            door.Unlock();

            // Remove the key
            keys.Remove(door.Hash);
        }
    }
}