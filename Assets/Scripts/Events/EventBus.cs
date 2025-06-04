using System;

namespace Events
{
    /// <summary>
    /// A generic, static event bus for publishing and subscribing to events of type T.
    /// Simplifies decoupled communication between different systems without direct references.
    /// </summary>
    /// <typeparam name="T">Type of the event data to be passed when raising the event.</typeparam>
    public static class EventBus<T>
    {
        // Internal event storing all subscribed listeners for event type T
        private static event Action<T> OnEvent;

        /// <summary>
        /// Subscribes a listener to the event of type T.
        /// </summary>
        /// <param name="listener">The method to be called when the event is raised.</param>
        public static void Subscribe(Action<T> listener)
        {
            OnEvent += listener;
        }

        /// <summary>
        /// Unsubscribes a listener from the event of type T.
        /// </summary>
        /// <param name="listener">The method to be removed from the event invocation list.</param>
        public static void Unsubscribe(Action<T> listener)
        {
            OnEvent -= listener;
        }

        /// <summary>
        /// Raises the event, notifying all subscribed listeners with the provided event data.
        /// </summary>
        /// <param name="eventData">Data associated with the event being raised.</param>
        public static void Raise(T eventData)
        {
            OnEvent?.Invoke(eventData);
        }
    }
}