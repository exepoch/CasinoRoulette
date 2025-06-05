namespace Events
{
    /// <summary>
    /// Event data structure representing a change in the player's bet action for each chip added.
    /// Contains the updated bet chip action count after betting or undoing the bet.
    /// </summary>
    public struct BetCountChangedEvent
    {
        /// <summary>
        /// The new count value after the change.
        /// </summary>
        public int BetCount;
    }
}