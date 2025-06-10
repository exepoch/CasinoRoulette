using Data;
using Events;
using Events.EventTypes;
using Events.EventTypes.Audio;
using UnityEngine;

namespace UI.ViewModels
{
    // ViewModel for the roulette game's main UI panel
    public class RouletteViewModel
    {
        // Bindable properties for UI updates
        public BindableProperty<long> Balance = new();                     // Current player balance
        public BindableProperty<long> Profit = new();                     // Current player profit
        public BindableProperty<long> TotalBet = new();                    // Total bet amount
        public BindableProperty<bool> CanUndo = new();                     // Whether undo is available
        public BindableProperty<bool> ButtonsEnabled = new();              // Whether UI buttons are interactable
        public BindableProperty<BetResultEvent> LastWinnings = new();         // Last winning amount and number
        public BindableProperty<GameState> StateChanged = new();           // Current game state

        // Subscribe to relevant game events
        public void Enable()
        {
            EventBus<BalanceChangedEvent>.Subscribe(OnBalanceChanged, true);
            EventBus<BetAmountChangedEvent>.Subscribe(OnBetAmountChanged, true);
            EventBus<BetCountChangedEvent>.Subscribe(OnBetCountChanged, true);
            EventBus<GameStateChangedEvent>.Subscribe(OnGameStateChanged, true);
            EventBus<BetResultEvent>.Subscribe(OnBetResult);
        }

        // Unsubscribe from events to prevent memory leaks
        public void Disable()
        {
            EventBus<BalanceChangedEvent>.Unsubscribe(OnBalanceChanged);
            EventBus<BetAmountChangedEvent>.Unsubscribe(OnBetAmountChanged);
            EventBus<BetCountChangedEvent>.Unsubscribe(OnBetCountChanged);
            EventBus<GameStateChangedEvent>.Unsubscribe(OnGameStateChanged);
            EventBus<BetResultEvent>.Unsubscribe(OnBetResult);
        }

        // Event handlers that update bindable properties
        private void OnBalanceChanged(BalanceChangedEvent e)
        {
            Balance.Value = e.UpdatedBalance;
            Profit.Value = e.UpdatedProfit;
        }

        private void OnBetAmountChanged(BetAmountChangedEvent e) => TotalBet.Value = e.UpdatedTotalBetAmount;

        private void OnBetCountChanged(BetCountChangedEvent e) => CanUndo.Value = e.BetCount > 0;

        private void OnGameStateChanged(GameStateChangedEvent e)
        {
            var interactable = e.CurrentState == GameState.Betting;
            ButtonsEnabled.Value = interactable;
            CanUndo.Value = interactable; // Undo is only allowed during betting
            StateChanged.Value = e.CurrentState;
        }

        private void OnBetResult(BetResultEvent e) =>
            LastWinnings.Value = e;

        // Command methods that raise corresponding events (UI button bindings)
        public void OnSpinClicked()
        {
            AudioEvents.RequestSound(SoundType.ButtonClick);
            EventBus<SpinButtonClickedEvent>.Raise(new SpinButtonClickedEvent());
        }

        public void OnUndoClicked()
        {
            AudioEvents.RequestSound(SoundType.ButtonClick);
            EventBus<UndoBetClickedEvent>.Raise(new UndoBetClickedEvent());
        }

        public void OnClearClicked()
        {
            AudioEvents.RequestSound(SoundType.ButtonClick);
            EventBus<ClearAllBetsEvent>.Raise(new ClearAllBetsEvent());
        }
    }
}
