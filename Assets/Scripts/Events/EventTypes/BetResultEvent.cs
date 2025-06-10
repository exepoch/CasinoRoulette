namespace Events.EventTypes
{
    /// <summary>
    /// Fired after bet result is calculated.
    /// </summary>
    public struct BetResultEvent
    {
        public long WinningAmount; // Total amount won.
        public int WinnerNumber; // The winner number.
    }
}