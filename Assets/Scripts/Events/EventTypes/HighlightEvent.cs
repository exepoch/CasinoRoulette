namespace Events
{
    /// <summary>
    /// Event used to request highlighting or clearing highlight on bet anchors (numbers).
    /// Contains the type of highlight action (Show or Hide) and the list of number IDs affected.
    /// </summary>
    public struct HighlightEvent
    {
        /// <summary>
        /// Defines whether to show or hide the highlight effect.
        /// </summary>
        public enum HighlightType { Show, Hide }

        /// <summary>
        /// The action type, either to show or hide highlights.
        /// </summary>
        public HighlightType Type;

        /// <summary>
        /// The array of number IDs that should be highlighted or cleared.
        /// </summary>
        public int[] NumberIds;
    }
}