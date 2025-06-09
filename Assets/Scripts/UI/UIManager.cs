using Events;
using Events.EventTypes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Chips
{
    /// <summary>
    /// Handles chip selection UI and related events.
    /// Provides global access through the singleton pattern.
    /// </summary>
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance { get; private set; }
        private static ChipUISelector selected;
        
        [Header("Interaction")]
        [SerializeField] private Button undoButton; // Button to undo the last bet.
        [SerializeField] private Button clearBetsButton; // Button to clear all placed bets.
        [SerializeField] private Button spinButton; // Button to start the spin.
        
        [Header("Displays")]
        [SerializeField] private TextMeshProUGUI balanceText; // Displays the player's current wallet balance.
        [SerializeField] private TextMeshProUGUI totalBetAmountText; // Displays the player's total bet amount.

        private void Awake()
        {
            // Set up singleton instance
            if (Instance != null && Instance != this)
            {
                Debug.LogWarning("Multiple UIManager instances found. Destroying duplicate.");
                Destroy(gameObject);
                return;
            }

            Instance = this;

            // Assign button click events
            undoButton.onClick.AddListener(() => EventBus<UndoBetClickedEvent>.Raise(new UndoBetClickedEvent()));
            clearBetsButton.onClick.AddListener(() => EventBus<ClearAllBetsEvent>.Raise(new ClearAllBetsEvent()));
            spinButton.onClick.AddListener(() => EventBus<SpinButtonClickedEvent>.Raise(new SpinButtonClickedEvent()));
        }

        private void OnEnable()
        {
            // Subscribe to game events
            EventBus<BalanceChangedEvent>.Subscribe(OnBalanceUpdate);
            EventBus<BetAmountChangedEvent>.Subscribe(OnBetAmountChanged);
            EventBus<BetCountChangedEvent>.Subscribe(OnBetCountChanged);
            EventBus<GameStateChangedEvent>.Subscribe(OnGameStateChanged);
            EventBus<BetResultEvent>.Subscribe(OnBetResult);
        }

        private void OnDisable()
        {
            // Unsubscribe from events to prevent memory leaks
            EventBus<BalanceChangedEvent>.Unsubscribe(OnBalanceUpdate);
            EventBus<BetAmountChangedEvent>.Unsubscribe(OnBetAmountChanged);
            EventBus<BetCountChangedEvent>.Unsubscribe(OnBetCountChanged);
            EventBus<GameStateChangedEvent>.Unsubscribe(OnGameStateChanged);
        }

        /// <summary>
        /// Called when a bet result is received.
        /// </summary>
        /// <param name="args">Bet result data.</param>
        private void OnBetResult(BetResultEvent args)
        {
            var isWon = args.WinningAmount > 0;
            
        }

        /// <summary>
        /// Updates the balance text UI.
        /// </summary>
        private void OnBalanceUpdate(BalanceChangedEvent eventArgs)
        {
            balanceText.text = $"Balance: ${eventArgs.UpdatedBalance:N0}";
        }

        /// <summary>
        /// Updates the total bet amount text UI.
        /// </summary>
        private void OnBetAmountChanged(BetAmountChangedEvent eventArgs)
        {
            totalBetAmountText.text = $"Total Bet: ${eventArgs.UpdatedTotalBetAmount:N0}";
        }

        /// <summary>
        /// Enables or disables the undo button based on current bet count.
        /// </summary>
        private void OnBetCountChanged(BetCountChangedEvent eventArgs)
        {
            undoButton.interactable = eventArgs.BetCount > 0;
        }

        /// <summary>
        /// Enables or disables UI buttons based on current game state.
        /// </summary>
        private void OnGameStateChanged(GameStateChangedEvent eventArgs)
        {
            var buttonsInteractable = eventArgs.CurrentState == GameState.Betting;
            undoButton.interactable = buttonsInteractable;
            clearBetsButton.interactable = buttonsInteractable;
            spinButton.interactable = buttonsInteractable;
        }

        /// <summary>
        /// Sets the newly selected chip and updates UI state.
        /// Broadcasts a ChipSelectedEvent.
        /// </summary>
        /// <param name="newSelector">The newly selected chip UI element.</param>
        public void SetNewSelector(ChipUISelector newSelector)
        {
            if (selected != null)
                selected.SetSelected(false);

            selected = newSelector;
            selected.SetSelected(true);

            EventBus<ChipSelectedEvent>.Raise(new ChipSelectedEvent
            {
                SelectedChip = selected.chipType
            });
        }
    }
}
