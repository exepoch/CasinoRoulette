using System.Collections.Generic;
using Events;
using Gameplay.Betting.Data;

namespace Gameplay.Betting
{
    /// <summary>
    /// Manages a stack of BetAction for undo and clear operations, raises events on changes.
    /// </summary>
    public class BetActionsPool
    {
        private readonly Stack<BetAction> _actions = new();

        /// <summary>
        /// Adds a new BetAction and raises bet count update event.
        /// </summary>
        public void Add(BetAnchor anchor, int value)
        {
            _actions.Push(new BetAction { Anchor = anchor, Value = value });
            EventBus<BetCountChangedEvent>.Raise(new BetCountChangedEvent { BetCount = _actions.Count });
        }
        
        /// <summary>
        /// Clears all actions and returns total bet value cleared.
        /// </summary>
        public int Clear()
        {
            int total = 0;
            foreach (var action in _actions)
                total += action.Value;

            _actions.Clear();
            return total;
        }

        /// <summary>
        /// Removes last action (undo) and raises event, returns popped action or null.
        /// </summary>
        public BetAction? Undo()
        {
            if (_actions.Count == 0) return null;

            EventBus<BetCountChangedEvent>.Raise(new BetCountChangedEvent { BetCount = _actions.Count - 1 });
            return _actions.Pop();
        }
    }
}