namespace Events.EventTypes
{
    /// <summary>
    /// Fired when total bet amount changes.
    /// </summary>
    public struct BetAmountChangedEvent
    {
        /// <summary>
        /// New total bet amount.
        /// </summary>
        public long UpdatedTotalBetAmount;
    }
}