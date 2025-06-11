using System;
using Events;
using Events.EventTypes;
using SubSystems.SaveSystem;
using UnityEngine;

namespace User
{
    /// <summary>
    /// Singleton Wallet class implementing IWalletService.
    /// Manages player balance with spending and adding funds,
    /// and notifies via events on balance changes.
    /// </summary>
    public class Wallet : MonoBehaviour, IWalletService,ISaveable<WalletDataSave>
    {
        public string SaveKey => "WalletCurrentBalance";
        private static IWalletService instance { get; set; }
        
        [SerializeField] private long startingBalance = 1000;

        private long _currentBalance;
        private long _overallProfit;

        private void Awake()
        {
            instance = this;

            _currentBalance = startingBalance;
            _overallProfit = _currentBalance - startingBalance;
            EventBus<BalanceChangedEvent>.Raise(new BalanceChangedEvent
            {
                UpdatedBalance = _currentBalance,
                UpdatedProfit = _overallProfit
            });
        }

        private void OnEnable()
        {
            EventBus<BetResultEvent>.Subscribe(OnWinResult);
        }
        private void OnDisable()
        {
            EventBus<BetResultEvent>.Unsubscribe(OnWinResult);
        }

        private void OnWinResult(BetResultEvent obj)
        {
            _overallProfit = _currentBalance - startingBalance;
        }

        /// <summary>
        /// Attempts to spend the specified amount.
        /// Returns false if balance insufficient.
        /// Raises BalanceChangedEvent if successful.
        /// </summary>
        public bool TrySpend(long amount)
        {
            if (_currentBalance < amount)
                return false;

            _currentBalance -= amount;
            _overallProfit = _currentBalance - startingBalance;

            EventBus<BalanceChangedEvent>.Raise(new BalanceChangedEvent
            {
                UpdatedBalance = _currentBalance,
                UpdatedProfit = _overallProfit
            });

            return true;
        }

        /// <summary>
        /// Adds funds to the wallet and raises BalanceChangedEvent.
        /// </summary>
        public void AddFunds(long amount)
        {
            _currentBalance += amount;
            _overallProfit = _currentBalance - startingBalance;
            EventBus<BalanceChangedEvent>.Raise(new BalanceChangedEvent
            {
                UpdatedBalance = _currentBalance,
                UpdatedProfit = _overallProfit
            });
        }

        /// <summary>
        /// Returns the current wallet balance.
        /// </summary>
        public long GetBalance()
        {
            return _currentBalance;
        }
        
        public WalletDataSave CaptureState()
        {
            return new WalletDataSave
            {
                lastBalance = _currentBalance,
                lastOverallProfit = _overallProfit
            };
        }

        public void RestoreState(WalletDataSave state)
        {
            _currentBalance = state.lastBalance;
            _overallProfit = state.lastOverallProfit;

            EventBus<BalanceChangedEvent>.Raise(new BalanceChangedEvent
            {
                UpdatedBalance = _currentBalance,
                UpdatedProfit = _overallProfit
            });
        }
        
    }
    [Serializable]
    public class WalletDataSave
    {
        public long lastBalance;
        public long lastOverallProfit;
    }
}
