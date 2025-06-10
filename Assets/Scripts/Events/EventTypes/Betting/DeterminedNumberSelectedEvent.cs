namespace Events.EventTypes
{
    /// <summary>
    /// Event data structure representing a change prededermined slot number.
    /// Contains the desired selected number.
    /// </summary>
    public struct DeterminedNumberSelectedEvent
    {
        public int SelectedNumber; // The new number value after the change.
    }
}