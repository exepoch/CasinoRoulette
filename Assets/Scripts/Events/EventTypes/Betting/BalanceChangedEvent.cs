namespace Events.EventTypes
{
    /// <summary>
    /// Event data structure representing a change in the player's wallet balance.
    /// Contains the updated balance after a transaction (spending or adding funds).
    /// </summary>
    public struct BalanceChangedEvent
    {
        /// <summary>
        /// The new balance value after the change.
        /// </summary>
        public long UpdatedBalance;
        public long UpdatedProfit;
    }
}