namespace Gameplay.Betting.Data
{
    /// <summary>
    /// Represents a bet placed by the player on a specific bet anchor.
    /// Tracks the anchor identifier and the total amount wagered on that anchor.
    /// </summary>
    public class PlacedBet
    {
        /// <summary>
        /// The unique identifier of the BetAnchor this bet is associated with.
        /// </summary>
        public int AnchorID;

        /// <summary>
        /// The total amount of chips placed on this bet.
        /// </summary>
        public int TotalAmount;
    }
}