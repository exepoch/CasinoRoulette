using UnityEngine;

namespace Core
{
    /// <summary>
    /// Provides a lazy-loaded singleton-like instance of a Unity Object type.
    /// </summary>
    /// <typeparam name="T">Type of the Unity Object to find or assign.</typeparam>
    public static class GenericInstanceProvider<T> where T : Object
    {
        private static T instance;

        /// <summary>
        /// Returns the cached instance or finds it in the scene if not set.
        /// </summary>
        public static T Get()
        {
            if (instance == null)
            {
                instance = Object.FindObjectOfType<T>();
                if (instance == null)
                    Debug.LogError($"{typeof(T).Name} not found in scene");
            }
            return instance;
        }

        /// <summary>
        /// Sets the instance manually.
        /// </summary>
        public static void Set(T customInstance)
        {
            instance = customInstance;
        }

        /// <summary>
        /// Resets the stored instance reference.
        /// </summary>
        public static void Reset()
        {
            instance = null;
        }
    }
}