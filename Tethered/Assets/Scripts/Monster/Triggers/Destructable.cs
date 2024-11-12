using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tethered.Monster.Triggers
{
    public class Destructable : MonoBehaviour
    {
        public enum DestroyType
        {
            True, // Destroy the GameObject
            Active // De-activate the GameObject
        }

        [SerializeField] private GameObject target;
        [SerializeField] private DestroyType type;

        /// <summary>
        /// Destruct the object
        /// </summary>
        public void Destruct()
        {
            // Destroy based on the type
            switch (type)
            {
                case DestroyType.True:
                    Destroy(target);
                    break;

                case DestroyType.Active:
                    target.SetActive(false);
                    break;
            }
        }
    }
}