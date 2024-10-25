using System.Collections.Generic;
using Tethered.Patterns.ServiceLocator;
using UnityEditor;
using UnityEngine;

namespace Tethered.Inventory
{
    public class SharedInventory : MonoBehaviour
    {
        [SerializeField] private HashSet<int> keys;
        [SerializeField] private List<bool> curtains;

        public HashSet<int> Keys { get => keys; }

        private void Awake()
        {
            // Register this as a service
            ServiceLocator.ForSceneOf(this).Register(this);

            // Initialize lists
            keys = new();
            curtains = new();
        }

        /// <summary>
        /// Get all of the Shared Inventory keys
        /// </summary>
        /// <returns></returns>
        public HashSet<int> GetKeys() => keys;

        /// <summary>
        /// Add a specific Key to the Shared Inventory
        /// </summary>
        public void StoreKey(int key)
        {
            keys.Add(key);

#if UNITY_EDITOR
            // Trigger for editor updates
            EditorUtility.SetDirty(this);
#endif
        }

        /// <summary>
        /// Check if the Shared Inventory has a specific Key
        /// </summary>
        public bool ContainsKey(int key) => keys.Contains(key);

        /// <summary>
        /// Remove a specific Key from the Shared Inventory
        /// </summary>
        public void RemoveKey(int key)
        {
            keys.Remove(key);

#if UNITY_EDITOR
            // Trigger for editor updates
            EditorUtility.SetDirty(this);
#endif
        }

        /// <summary>
        /// Add a Curtain to the Shared Inventory
        /// </summary>
        public void StoreCurtain() => curtains.Add(true);

        /// <summary>
        /// Check if the Shared Inventory has a Curtain
        /// </summary>
        public bool ContainsCurtain() => curtains.Count > 0;

        /// <summary>
        /// Remove a Curtain from the Shared Inventory
        /// </summary>

        public void RemoveCurtain() => curtains.RemoveAt(0);
    }
}