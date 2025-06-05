using System;
using System.Collections.Generic;
using System.Linq;
using Events;
using Gameplay.Betting.Data;
using Gameplay.Betting.Interfaces;
using UnityEngine;
using User;

namespace Gameplay.Betting
{
    /// <summary>
    /// Manages player bets: placing, undoing, clearing bets.
    /// Uses wallet and anchor services, listens to relevant events.
    /// </summary>
    public class BetManager : MonoBehaviour
    {
        private long _totalBetAmount;
        private BetActionsPool _betActionsPool;
        private List<PlacedBet> activeBets = new();
        private IAnchorService _anchorService;
        private IWalletService _walletService;
        private int _currentSelectedChip;

        private void Awake()
        {
            _walletService = Wallet.Instance ?? throw new Exception("Wallet instance missing!");
            _anchorService = AnchorManager.Instance ?? throw new Exception("AnchorManager instance missing!");
            _betActionsPool = new BetActionsPool();
        }

        private void OnEnable()
        {
            EventBus<ChipSelectedEvent>.Subscribe(OnChipSelected);
            EventBus<BetAnchorClickedEvent>.Subscribe(OnBetAnchorClicked);
            EventBus<UndoBetClickedEvent>.Subscribe(OnUndoBetClicked);
            EventBus<ClearAllBetsEvent>.Subscribe(OnClearAllBets);
        }

        private void OnDisable()
        {
            EventBus<ChipSelectedEvent>.Unsubscribe(OnChipSelected);
            EventBus<BetAnchorClickedEvent>.Unsubscribe(OnBetAnchorClicked);
            EventBus<UndoBetClickedEvent>.Unsubscribe(OnUndoBetClicked);
            EventBus<ClearAllBetsEvent>.Unsubscribe(OnClearAllBets);
        }

        private void OnChipSelected(ChipSelectedEvent evt) => _currentSelectedChip = (int)evt.SelectedChip;

        private void OnBetAnchorClicked(BetAnchorClickedEvent evt) => PlaceBet(evt.ClickedAnchorId);

        private void OnUndoBetClicked(UndoBetClickedEvent evt) => UndoBet();

        private void OnClearAllBets(ClearAllBetsEvent evt) => ClearAllBets();

        private void PlaceBet(int anchorID)
        {
            if (!_walletService.TrySpend(_currentSelectedChip)) return;

            var existing = activeBets.FirstOrDefault(b => b.AnchorID == anchorID);
            if (existing != null) existing.TotalAmount += _currentSelectedChip;
            else activeBets.Add(new PlacedBet { AnchorID = anchorID, TotalAmount = _currentSelectedChip });

            var anchor = _anchorService.GetAnchorById(anchorID);
            if (anchor == null) return;

            anchor.AddChips(_currentSelectedChip);
            _betActionsPool.Add(anchor, _currentSelectedChip);

            _totalBetAmount = activeBets.Sum(b => b.TotalAmount);
            EventBus<BetAmountChangedEvent>.Raise(new BetAmountChangedEvent { UpdatedTotalBetAmount = _totalBetAmount });
        }

        private void UndoBet()
        {
            var action = _betActionsPool.Undo();
            if (!action.HasValue) return;

            var betValue = action.Value.Value;
            action.Value.Anchor.RemoveChips(betValue);
            _totalBetAmount -= betValue;
            _walletService.AddFunds(betValue);

            EventBus<BetAmountChangedEvent>.Raise(new BetAmountChangedEvent { UpdatedTotalBetAmount = _totalBetAmount });
        }

        private void ClearAllBets()
        {
            foreach (var anchor in _anchorService.GetAll())
                anchor?.ClearBets();

            activeBets.Clear();
            _walletService.AddFunds(_betActionsPool.Clear());
            _totalBetAmount = 0;

            EventBus<BetAmountChangedEvent>.Raise(new BetAmountChangedEvent { UpdatedTotalBetAmount = _totalBetAmount });
        }
    }
}
