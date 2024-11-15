using UnityEngine;

namespace Tethered.Extensions.GameObjects
{
    public static class GameObjectExtensions
    {
        /// <summary>
        /// Checks if a GameObject has a component, and if it doesn't, add one
        /// </summary>
        public static T GetOrAdd<T>(this GameObject gameObject) where T : Component
        {
            T component = gameObject.GetComponent<T>();
            if (!component) component = gameObject.AddComponent<T>();

            return component;
        }

        /// <summary>
        /// Returns the object itself if it exists, null otherwise
        /// </summary>
        public static T OrNull<T>(this T obj) where T : Object => obj ? obj : null;
    }
}