using Tethered.Interactables.Events;
using Tethered.Interactables;
using Tethered.Patterns.EventBus;
using Tethered.Patterns.ServiceLocator;
using UnityEngine;

namespace Tethered.Inventory
{
    public class PlayerInventory : MonoBehaviour
    {
        private SharedInventory inventory;

        // Start is called before the first frame update
        void Start()
        {
            // Get the shared inventory
            inventory = ServiceLocator.ForSceneOf(this).Get<SharedInventory>();
        }

        /// <summary>
        /// Add a specific Key to the Shared Inventory
        /// </summary>
        public void StoreKey(int key) => inventory.StoreKey(key);

        /// <summary>
        /// Add a Curtain to the Shared Inventory
        /// </summary>
        public void StoreCurtain() => inventory.StoreCurtain();

        /// <summary>
        /// Check a Key in the Player's key set
        /// </summary>
        public bool CheckKey(Keyhole keyhole)
        {
            // Exit case - if the Key is not found
            if (!inventory.ContainsKey(keyhole.Hash)) return false;

            return true;
        }

        /// <summary>
        /// Remove a Key from the Shared Inventory
        /// </summary>
        public void RemoveKey(int hash) => inventory.RemoveKey(hash);

        public bool CheckCurtain(Window window)
        {
            // Exit case - if a Curtain is not stored
            if (!inventory.ContainsCurtain()) return false;

            // Cover the window
            window.CoverWindow();

            // Remove the Curtain from the inventory
            inventory.RemoveCurtain();

            return true;
        }
    }
}