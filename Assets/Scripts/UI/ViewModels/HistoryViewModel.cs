using System;
using System.Collections.Generic;
using Events;
using Events.EventTypes;
using UI.Models;

namespace UI.ViewModels
{
    /// <summary>
    /// ViewModel for managing roulette history entries.
    /// Handles data updates and communicates with the view via events.
    /// </summary>
    public class HistoryViewModel
    {
        // List of history entries (most recent at the top)
        public List<HistoryEntryModel> Entries = new();

        // Event triggered when the history list is updated
        public event Action OnEntriesUpdated;

        /// <summary>
        /// Adds a new entry to the history and notifies listeners.
        /// </summary>
        public void AddEntry(int number, string color)
        {
            // Insert at the top of the list
            Entries.Insert(0, new HistoryEntryModel { Number = number, Color = color });
            OnEntriesUpdated?.Invoke();
        }

        /// <summary>
        /// Registers this ViewModel to listen to BetResultEvents from the EventBus.
        /// </summary>
        public void RegisterEvents()
        {
            EventBus<BetResultEvent>.Subscribe(OnBetResult);
        }

        /// <summary>
        /// Called when a bet result is published. Adds the result to the history.
        /// </summary>
        private void OnBetResult(BetResultEvent args)
        {
            //Green if there's a win, otherwise red
            AddEntry(args.WinnerNumber, args.WinningAmount > 0 ? "Green" : "Red");
        }
    }
}