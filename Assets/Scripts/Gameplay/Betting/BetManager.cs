using System.Collections.Generic;
using System.Linq;
using Data;
using Events;
using Events.EventTypes;
using Events.EventTypes.Audio;
using Gameplay.Betting.Data;
using Gameplay.Betting.Interfaces;
using SubSystems.SaveSystem;
using UnityEngine;
using User;

namespace Gameplay.Betting
{
    /// <summary>
    /// Manages player bets: placing, undoing, clearing bets.
    /// Uses wallet and anchor services, listens to relevant events.
    /// </summary>
    public class BetManager : MonoBehaviour,ISaveable<BetManagerSaveData>
    {
        public string SaveKey => "BetManagerSaveData";
        private long _totalBetAmount;
        private BetActionsPool _betActionsPool;
        private List<PlacedBet> activeBets = new();
        private IAnchorService _anchorService;
        private IWalletService _walletService;
        private long _currentSelectedChip;

        private void Awake()
        {
            _walletService = Wallet.Instance;
            _anchorService = AnchorManager.Instance;
            _betActionsPool = new BetActionsPool();
        }

        private void OnEnable()
        {
            EventBus<ChipSelectedEvent>.Subscribe(OnChipSelected,true);
            EventBus<BetAnchorClickedEvent>.Subscribe(OnBetAnchorClicked);
            EventBus<UndoBetClickedEvent>.Subscribe(OnUndoBetClicked);
            EventBus<ClearAllBetsEvent>.Subscribe(OnClearAllBets);
            EventBus<BallStoppedEvent>.Subscribe(OnBallStopped);
        }
        private void OnDisable()
        {
            EventBus<ChipSelectedEvent>.Unsubscribe(OnChipSelected);
            EventBus<BetAnchorClickedEvent>.Unsubscribe(OnBetAnchorClicked);
            EventBus<UndoBetClickedEvent>.Unsubscribe(OnUndoBetClicked);
            EventBus<ClearAllBetsEvent>.Unsubscribe(OnClearAllBets);
            EventBus<BallStoppedEvent>.Unsubscribe(OnBallStopped);
        }
        private void OnBallStopped(BallStoppedEvent obj)
        {
            var totalWinning = _anchorService.GetAll().Sum(x => x.Winnings(obj.SlotNumberCount,obj.ResultNumber));
            _walletService.AddFunds(totalWinning);
            EventBus<BetResultEvent>.Raise(new BetResultEvent
            {
                WinningAmount = totalWinning,
                WinnerNumber = obj.ResultNumber,
                LoseAmount = totalWinning >0 ? 0 : _totalBetAmount
            });
            AudioEvents.RequestSound(totalWinning > 0 ? SoundType.Win : SoundType.Lose);
            ClearAllBets(true);
        }

        private void OnChipSelected(ChipSelectedEvent evt)
        {
            _currentSelectedChip = (int)evt.SelectedChip;
        }

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
            AudioEvents.RequestSound(SoundType.BetPlaced);
        }

        private void UndoBet()
        {
            var action = _betActionsPool.Undo();
            if (!action.HasValue) return;

            var betValue = action.Value.Value;
            action.Value.Anchor.RemoveChips(betValue);
            _totalBetAmount -= betValue;
            _walletService.AddFunds(betValue);
            var existing = activeBets.FirstOrDefault(b => b.AnchorID == action.Value.Anchor.AnchorID);
            if (existing != null)
            {
                existing.TotalAmount -= betValue;
                if (existing.TotalAmount <= 0)
                    activeBets.Remove(existing);
            }

            EventBus<BetAmountChangedEvent>.Raise(new BetAmountChangedEvent { UpdatedTotalBetAmount = _totalBetAmount });
            AudioEvents.RequestSound(SoundType.BetPlaced);
        }

        private void ClearAllBets(bool fromResult = false)
        {
            foreach (var anchor in _anchorService.GetAll())
                anchor?.ClearBets();

            activeBets.Clear();
            var returningBalance = _betActionsPool.Clear();
            if(!fromResult)
                _walletService.AddFunds(returningBalance);
            _totalBetAmount = 0;

            EventBus<BetAmountChangedEvent>.Raise(new BetAmountChangedEvent { UpdatedTotalBetAmount = _totalBetAmount });
            AudioEvents.RequestSound(SoundType.BetPlaced);
        }

        
        public BetManagerSaveData CaptureState()
        {
            var data = new BetManagerSaveData();
            data.betActionActiveBetsSaves = new List<BetActionActiveBetsSave>();

            foreach (var action in activeBets)  
            {
                data.betActionActiveBetsSaves.Add(new BetActionActiveBetsSave
                {
                    anchorID = action.AnchorID,
                    TotalAmount = action.TotalAmount
                });
            }

            data.betActionSaves = new List<BetActionSave>();
            foreach (var betAction in _betActionsPool.GetActions().Reverse())
            {
                data.betActionSaves.Add(new BetActionSave
                {
                    anchorID = betAction.Anchor.AnchorID,
                    selectedChip = betAction.Value
                });
            }
            
            data.currentSelectedChip = _currentSelectedChip;
            data.totalBetAmount = _totalBetAmount;
            return data;
        }

        public void RestoreState(BetManagerSaveData state)
        {
            _currentSelectedChip = state.currentSelectedChip;
            _totalBetAmount = state.totalBetAmount;
            EventBus<BetAmountChangedEvent>.Raise(new BetAmountChangedEvent { UpdatedTotalBetAmount = _totalBetAmount });
            
            foreach (var actionSave in state.betActionSaves)
            {
                var anchor = _anchorService.GetAnchorById(actionSave.anchorID);
                if (anchor == null) continue;
                _betActionsPool.Add(anchor, actionSave.selectedChip);
            }

            foreach (var betSave in state.betActionActiveBetsSaves)
            {
                activeBets.Add(new PlacedBet { AnchorID = betSave.anchorID, TotalAmount = betSave.TotalAmount });
            }
            EventBus<ChipSelectedEvent>.Raise(new ChipSelectedEvent
            {
                SelectedChip = (ChipType)_currentSelectedChip
            });
        }
    }
    
    [System.Serializable]
    public struct BetManagerSaveData
    {
        public List<BetActionSave> betActionSaves;
        public List<BetActionActiveBetsSave> betActionActiveBetsSaves;
        public long currentSelectedChip;
        public long totalBetAmount;
    }

    [System.Serializable]
    public struct BetActionSave
    {
        public int anchorID;
        public long selectedChip;
    }
    [System.Serializable]
    public struct BetActionActiveBetsSave
    {
        public int anchorID;
        public long TotalAmount;
    }
}
