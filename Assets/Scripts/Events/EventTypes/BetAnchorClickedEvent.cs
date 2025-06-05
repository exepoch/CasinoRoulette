namespace Events
{
    /// <summary>
    /// Event data structure dispatched when a BetAnchor (betting position) is clicked by the player.
    /// Carries the ID of the clicked anchor to identify which betting spot was selected.
    /// </summary>
    public struct BetAnchorClickedEvent
    {
        /// <summary>
        /// The unique identifier of the clicked bet anchor.
        /// </summary>
        public int ClickedAnchorId;
    }
}