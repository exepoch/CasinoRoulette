using System.Collections.Generic;

namespace Gameplay.Betting.Interfaces
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

        /// <summary>
        /// Returns all registered betanchors with list.
        /// </summary>
        public List<BetAnchor> GetAll();
    }
}
