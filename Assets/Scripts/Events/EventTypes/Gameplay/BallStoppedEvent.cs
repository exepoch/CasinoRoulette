namespace Events.EventTypes
{
    /// <summary>
    /// Event triggered when the roulette ball stops spinning.
    /// </summary>
    public struct BallStoppedEvent
    {
        public int ResultNumber; // The final number where the ball landed.
        public int SlotNumberCount; // Total number of slots on the roulette wheel (e.g., 37 for European, 38 for American).
    }
}