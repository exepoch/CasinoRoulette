namespace Events.EventTypes
{
    /// <summary>
    /// Fired when the game state changes.
    /// </summary>
    public struct GameStateChangedEvent
    {
        /// <summary>
        /// New game state.
        /// </summary>
        public GameState CurrentState;
    }
}