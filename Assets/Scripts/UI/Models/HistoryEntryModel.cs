namespace UI.Models
{
    /// <summary>
    /// Data model representing a single roulette history entry.
    /// </summary>
    public class HistoryEntryModel
    {
        public int Number;     // The rolled number (e.g. 17)
        public string Color;   // The color associated with the number ("Red", "Green", etc.)
    }
}