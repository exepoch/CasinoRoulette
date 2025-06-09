using Events;
using Events.EventTypes;
using UnityEngine;

namespace Gameplay.Manager
{
    /// <summary>
    /// Manages the main roulette game loop and state transitions.
    /// </summary>
    public class RouletteManager : MonoBehaviour
    {
        public static RouletteManager Instance;
        
        private GameState _currentState;

        private void Awake()
        {
            Instance = this;
            SetState(GameState.Betting);
        }

        private void OnEnable()
        {
            EventBus<SpinButtonClickedEvent>.Subscribe(@event => TrySpin());
            EventBus<BallStoppedEvent>.Subscribe(OnBallStopped);
        }

        private void OnDisable()
        {
            EventBus<SpinButtonClickedEvent>.Unsubscribe(@event => TrySpin());
            EventBus<BallStoppedEvent>.Unsubscribe(OnBallStopped);
        }

        /// <summary>
        /// Called when the ball stops. Triggers result state and restarts cycle.
        /// </summary>
        private void OnBallStopped(BallStoppedEvent args)
        {
            SetState(GameState.Result);
            Invoke(nameof(BeginGameCycle), 2);
        }

        /// <summary>
        /// Updates the current game state and notifies listeners.
        /// </summary>
        private void SetState(GameState newState)
        {
            _currentState = newState;
            EventBus<GameStateChangedEvent>.Raise(new GameStateChangedEvent
            {
                CurrentState = _currentState
            });
        }

        /// <summary>
        /// Resets the game to the betting phase.
        /// </summary>
        private void BeginGameCycle()
        {
            SetState(GameState.Betting);
        }

        /// <summary>
        /// Attempts to start spinning if in the betting state.
        /// </summary>
        private void TrySpin()
        {
            if (_currentState != GameState.Betting) return;
            SetState(GameState.Spinning);
        }
    }
}