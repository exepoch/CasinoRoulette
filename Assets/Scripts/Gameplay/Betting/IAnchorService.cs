namespace Gameplay.Betting
{
    public interface IAnchorService
    {
        /// <summary>
        /// Gets the BetAnchor instance by its ID.
        /// Throws if no anchor with given ID exists.
        /// </summary>
        /// <param name="id">Anchor ID to fetch.</param>
        /// <returns>BetAnchor instance.</returns>
        BetAnchor GetAnchorById(int id);
    }
}
