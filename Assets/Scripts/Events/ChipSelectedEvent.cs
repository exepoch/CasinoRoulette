using Data;

namespace Events
{
    /// <summary>
    /// Event data structure raised when the player selects a chip denomination.
    /// Contains the type/value of the selected chip.
    /// </summary>
    public struct ChipSelectedEvent
    {
        /// <summary>
        /// The chip type that the player selected for betting.
        /// </summary>
        public ChipType SelectedChip;
    }
}