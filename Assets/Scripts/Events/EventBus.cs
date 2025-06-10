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
        private static T lastEvent;
        private static bool hasEvent = false;

        /// <summary>
        /// Subscribes a listener to the event of type T.
        /// </summary>
        /// <param name="subscriber">The method to be called when the event is raised.</param>
        /// <param name="receiveLastEventImmediately">
        /// If true, the subscriber will immediately receive the most recently raised event of type T
        /// (if any) upon subscription. This is useful for state-like events where the latest value
        /// should be delivered right away. For one-time or transient events, set this to false.
        /// </param>
        public static void Subscribe(Action<T> subscriber, bool receiveLastEventImmediately = false)
        {
            OnEvent += subscriber;
            if (receiveLastEventImmediately && hasEvent)
                subscriber(lastEvent);
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
            lastEvent = eventData;
            hasEvent = true;
            OnEvent?.Invoke(eventData);
        }
    }
}