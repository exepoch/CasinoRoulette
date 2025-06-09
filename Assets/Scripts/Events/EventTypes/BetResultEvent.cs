namespace Events.EventTypes
{
    /// <summary>
    /// Fired after bet result is calculated.
    /// </summary>
    public struct BetResultEvent
    {
        /// <summary>
        /// Total amount won.
        /// </summary>
        public long WinningAmount;
    }
}