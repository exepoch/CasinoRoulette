namespace Data
{
    /// <summary>
    /// Represents the different denominations of betting chips available in the game.
    /// Each enum value corresponds to the chip's monetary value.
    /// </summary>
    public enum ChipType
    {
        None = -1,          //None selected
        One = 1,           // Chip worth 1 unit
        Five = 5,          // Chip worth 5 units
        Ten = 10,          // Chip worth 10 units
        Fifty = 50,        // Chip worth 50 units
        Hundred = 100,     // Chip worth 100 units
        FiveHundred = 500  // Chip worth 500 units
    }
}