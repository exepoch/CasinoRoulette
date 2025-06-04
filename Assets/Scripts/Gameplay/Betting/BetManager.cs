using System.Collections.Generic;
using System.Linq;
using Events;
using Gameplay.Betting.Data;
using UnityEngine;
using User;

namespace Gameplay.Betting
{
    /// <summary>
    /// Manages bets placed by the player.
    /// Subscribes to chip selection and anchor click events to handle placing bets.
    /// Uses an abstracted wallet service to check and update player balance.
    /// </summary>
    public class BetManager : MonoBehaviour
    {
        [Tooltip("Reference to the Wallet component to manage player balance.")]
        [SerializeField] private Wallet wallet;
        [SerializeField] private AnchorManager anchorManager;

        private long _totalBetAmount;
        
        // Currently active bets on anchors
        private List<PlacedBet> activeBets = new();
        private IAnchorService _anchorService;

        // The currently selected chip value
        private int _currentSelectedChip;

        // Abstract wallet service to allow for better testability and loose coupling
        private IWalletService _walletService;

        private void Awake()
        {
            // Initialize wallet service from serialized Wallet instance
            _walletService = wallet;
            _anchorService = anchorManager;
        }

        private void OnEnable()
        {
            // Subscribe to relevant events
            EventBus<ChipSelectedEvent>.Subscribe(OnChipSelected);
            EventBus<BetAnchorClickedEvent>.Subscribe(OnBetAnchorClicked);
        }

        private void OnDisable()
        {
            // Unsubscribe to prevent memory leaks and unexpected behavior
            EventBus<ChipSelectedEvent>.Unsubscribe(OnChipSelected);
            EventBus<BetAnchorClickedEvent>.Unsubscribe(OnBetAnchorClicked);
        }

        /// <summary>
        /// Handles BetAnchor click event by placing a bet on the clicked anchor ID.
        /// </summary>
        private void OnBetAnchorClicked(BetAnchorClickedEvent obj) => PlaceBet(obj.ClickedAnchorId);

        /// <summary>
        /// Updates the current selected chip value based on chip selection event.
        /// </summary>
        private void OnChipSelected(ChipSelectedEvent eventArg)
        {
            _currentSelectedChip = (int)eventArg.SelectedChip;
        }

        /// <summary>
        /// Attempts to place a bet on the given anchor ID using the currently selected chip.
        /// Checks wallet balance before placing the bet.
        /// </summary>
        private void PlaceBet(int anchorID)
        {
            // Check if player has enough funds
            if (!_walletService.TrySpend(_currentSelectedChip)) 
                return;

            // Find existing bet on this anchor if any
            var existing = activeBets.FirstOrDefault(b => b.AnchorID == anchorID);

            if (existing != null)
            {
                // Add chip amount to existing bet
                existing.TotalAmount += _currentSelectedChip;
            }
            else
            {
                // Add a new bet entry
                activeBets.Add(new PlacedBet
                {
                    AnchorID = anchorID,
                    TotalAmount = _currentSelectedChip
                });
            }

            // Notify BetAnchor to update visual chips stack
            _anchorService.GetAnchorById(anchorID)?.AddChips(_currentSelectedChip);
            _totalBetAmount = activeBets.Sum(x => x.TotalAmount);
            EventBus<BetAmountChangedEvent>.Raise(new BetAmountChangedEvent
            {
                UpdatedTotalBetAmount = _totalBetAmount
            });
        }

        /// <summary>
        /// Clears all active bets and resets chips on each anchor.
        /// </summary>
        public void ClearAllBets()
        {
            foreach (var bet in activeBets)
                _anchorService.GetAnchorById(bet.AnchorID)?.ClearBets();

            activeBets.Clear();
        }
    }
}
