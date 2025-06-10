using Data;
using Events;
using Events.EventTypes;
using Events.EventTypes.Audio;
using UnityEngine;

namespace UI.Chips
{
    // ViewModel for a single chip selector button
    public class ChipSelectorViewModel
    {
        // Currently selected chip type (shared across all selectors)
        public static BindableProperty<ChipType> SelectedChipType = new()
        {
            Value = ChipType.None
        };

        // Whether this chip can be interacted with in the UI
        public BindableProperty<bool> StateInteractable = new();

        // Whether this chip is currently bettable (based on balance)
        public BindableProperty<bool> Bettable = new();

        private readonly ChipType _chipType;
        private long _currentBalance;
        private GameState _currentGameState = GameState.Betting;

        public ChipSelectorViewModel(ChipType chipType)
        {
            _chipType = chipType;
        }

        // Called when this view model is enabled (e.g. UI is shown)
        public void Enable()
        {
            // Listen for balance and game state changes
            EventBus<BalanceChangedEvent>.Subscribe(OnBalanceUpdate, true);
            EventBus<GameStateChangedEvent>.Subscribe(OnGameStateChanged, true);
            EventBus<ChipSelectedEvent>.Subscribe(OnChipSelected, true);
            if (SelectedChipType.Value == ChipType.None)
            {
                SelectedChipType.Value = ChipType.One;
                EventBus<ChipSelectedEvent>.Raise(new ChipSelectedEvent
                {
                    SelectedChip = ChipType.One
                });
            }
                
        }

        // Called when this view model is disabled (e.g. UI is hidden)
        public void Disable()
        {
            // Stop listening to avoid memory leaks
            EventBus<BalanceChangedEvent>.Unsubscribe(OnBalanceUpdate);
            EventBus<GameStateChangedEvent>.Unsubscribe(OnGameStateChanged);
            EventBus<ChipSelectedEvent>.Unsubscribe(OnChipSelected);
        }

        // Triggered when this chip is selected by the user
        public void OnSelectChipType(ChipType chipType)
        {
            AudioEvents.RequestSound(SoundType.ButtonClick);
            // Notify the system about the selection
            EventBus<ChipSelectedEvent>.Raise(new ChipSelectedEvent
            {
                SelectedChip = chipType
            });
        }
        
        private void OnChipSelected(ChipSelectedEvent obj)
        {
            SelectedChipType.Value = obj.SelectedChip;
        }

        // React to balance changes
        private void OnBalanceUpdate(BalanceChangedEvent e)
        {
            _currentBalance = e.UpdatedBalance;
            ReevaluateProperties();
        }

        // React to game state changes
        private void OnGameStateChanged(GameStateChangedEvent e)
        {
            _currentGameState = e.CurrentState;
            ReevaluateProperties();
        }

        // Update viewmodel properties based on balance and game state
        private void ReevaluateProperties()
        {
            var canBet = _currentBalance >= (int)_chipType;

            Bettable.Value = canBet;
            StateInteractable.Value = canBet && _currentGameState == GameState.Betting;
        }
    }
}
