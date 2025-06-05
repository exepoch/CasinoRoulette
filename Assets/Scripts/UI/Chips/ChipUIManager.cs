using System;
using Events;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Chips
{
    /// <summary>
    /// Manages the current selected chip UI and broadcasts chip selection events.
    /// Implements singleton pattern for global access.
    /// </summary>
    public class ChipUIManager : MonoBehaviour
    {
        /// <summary>
        /// Singleton instance of the ChipUIManager.
        /// </summary>
        public static ChipUIManager Instance { get; private set; }
        private static ChipUISelector selected;

        
    
        /// <summary>
        /// Reference to the UI Text element displaying the player's current wallet balance.
        /// This should be assigned via the Inspector with a TextMeshProUGUI component.
        /// </summary>
        [SerializeField] private TextMeshProUGUI balanceText;
        /// <summary>
        /// Reference to the UI Text element displaying the player's current total bet amount.
        /// This should be assigned via the Inspector with a TextMeshProUGUI component.
        /// </summary>
        [SerializeField] private TextMeshProUGUI totalBetAmountText;
        
        [SerializeField] private Button undoButton;
        [SerializeField] private Button clearBetsButton;


        private void Awake()
        {
            // Ensure singleton instance assignment
            if (Instance != null && Instance != this)
            {
                Debug.LogWarning("Multiple instances of ChipUIManager detected. Destroying duplicate.");
                Destroy(gameObject);
                return;
            }
            Instance = this;
            
            undoButton.onClick.AddListener(()=> EventBus<UndoBetClickedEvent>.Raise(new UndoBetClickedEvent()));
            clearBetsButton.onClick.AddListener(()=> EventBus<ClearAllBetsEvent>.Raise(new ClearAllBetsEvent()));
        }
        
        private void OnEnable()
        {
            // Subscribe to balance and bet amount change events to update bettable state
            EventBus<BalanceChangedEvent>.Subscribe(OnBalanceUpdate);
            EventBus<BetAmountChangedEvent>.Subscribe(OnBetAmountChanged);
            EventBus<BetCountChangedEvent>.Subscribe(OnBetCountChanged);
        }

        private void OnDisable()
        {
            // Unsubscribe when disabled to avoid memory leaks
            EventBus<BalanceChangedEvent>.Unsubscribe(OnBalanceUpdate);
            EventBus<BetAmountChangedEvent>.Unsubscribe(OnBetAmountChanged);
            EventBus<BetCountChangedEvent>.Unsubscribe(OnBetCountChanged);
        }
        
        private void OnBalanceUpdate(BalanceChangedEvent obj)
        {
            balanceText.text = $"Balance: ${obj.UpdatedBalance:N0}";
        }
        private void OnBetAmountChanged(BetAmountChangedEvent obj)
        {
            totalBetAmountText.text = $"Total Bet: ${obj.UpdatedTotalBetAmount:N0}";
        }
        private void OnBetCountChanged(BetCountChangedEvent obj)
        {
            undoButton.interactable = obj.BetCount > 0;
        }
        
        /// <summary>
        /// Updates the currently selected chip UI selector.
        /// Raises a ChipSelectedEvent with the new selection.
        /// </summary>
        /// <param name="newSelector">The new ChipUISelector selected by user.</param>
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