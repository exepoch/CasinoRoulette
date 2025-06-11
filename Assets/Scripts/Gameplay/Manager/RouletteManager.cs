using System;
using Data;
using Events;
using Events.EventTypes;
using Events.EventTypes.Audio;
using SubSystems.SaveSystem;
using UnityEngine;
using Utils;

namespace Gameplay.Manager
{
    /// <summary>
    /// Manages the main roulette game loop and state transitions.
    /// </summary>
    public class RouletteManager : MonoBehaviour,ISaveable<GameState>
    {
        [SerializeField] private SceneHideCG cg;
        public string SaveKey => "RouletteManagerGameStateSave";

        [SerializeField] private CameraManager cameraManager;
        private GameState _currentState;

        private void Awake()
        {
            SetState(GameState.Betting);
            cameraManager.RotateToBetTable(0);
        }

        private void OnEnable()
        {
            EventBus<SpinButtonClickedEvent>.Subscribe(SpinButtonClicked);
            EventBus<BallStoppedEvent>.Subscribe(OnBallStopped);
            EventBus<DataLoadedEvent>.Subscribe(OnDataLoaded);
        }

        private void SpinButtonClicked(SpinButtonClickedEvent obj)
        {
            TrySpin();
        }

        private void OnDisable()
        {
            EventBus<SpinButtonClickedEvent>.Unsubscribe(SpinButtonClicked);
            EventBus<BallStoppedEvent>.Unsubscribe(OnBallStopped);
            EventBus<DataLoadedEvent>.Unsubscribe(OnDataLoaded);
        }

        /// <summary>
        /// Called when the ball stops. Triggers result state and restarts cycle.
        /// </summary>
        private void OnBallStopped(BallStoppedEvent args)
        {
            SetState(GameState.Result);
            Invoke(nameof(BeginGameCycle), 2);
        }
        
        private void OnDataLoaded(DataLoadedEvent obj)
        {
            // Start the UI fade-out animation after loading
            cg.FadeOut(2);
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
            cameraManager.RotateToBetTable(2, callBack: () => SetState(GameState.Betting));
        }

        /// <summary>
        /// Attempts to start spinning if in the betting state.
        /// </summary>
        private void TrySpin()
        {
            if (_currentState != GameState.Betting) return;
            AudioEvents.RequestSound(SoundType.SpinStart);
            SetState(GameState.Spinning);
            cameraManager.RotateToRoulette(3,.5f);
        }

        
        public GameState CaptureState()
        {
            return _currentState;
        }

        public void RestoreState(GameState state)
        {
            _currentState = state;
            SetState(GameState.Betting);
        }
    }
}