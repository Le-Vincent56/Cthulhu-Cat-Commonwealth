using System;
using UnityEngine;
namespace Tethered.World
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

        public event Action OnDestruct = delegate { };

        /// <summary>
        /// Destruct the object
        /// </summary>
        public void Destruct()
        {
            // Invoke the destruct event
            OnDestruct.Invoke();

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