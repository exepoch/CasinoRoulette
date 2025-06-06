using Events;
using UnityEngine;

namespace User
{
    /// <summary>
    /// Singleton Wallet class implementing IWalletService.
    /// Manages player balance with spending and adding funds,
    /// and notifies via events on balance changes.
    /// </summary>
    public class Wallet : MonoBehaviour, IWalletService
    {
        private static IWalletService instance { get; set; }

        public static IWalletService Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindFirstObjectByType<Wallet>();

                    if (instance != null) return instance;
                    var go = new GameObject("Wallet");
                    instance = go.AddComponent<Wallet>();
                }
                return instance;
            }
        }
        
        [SerializeField]
        private long startingBalance = 1000;

        private long currentBalance;

        private void Awake()
        {
            // Singleton pattern to ensure only one instance exists
            if (Instance != null && (Wallet)Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            instance = this;
            DontDestroyOnLoad(gameObject);

            currentBalance = startingBalance;
        }

        /// <summary>
        /// Attempts to spend the specified amount.
        /// Returns false if balance insufficient.
        /// Raises BalanceChangedEvent if successful.
        /// </summary>
        public bool TrySpend(long amount)
        {
            if (currentBalance < amount)
                return false;

            currentBalance -= amount;
            Debug.LogWarning($"Spent {amount}. Current Balance: {currentBalance}");

            EventBus<BalanceChangedEvent>.Raise(new BalanceChangedEvent
            {
                UpdatedBalance = currentBalance
            });

            return true;
        }

        /// <summary>
        /// Adds funds to the wallet and raises BalanceChangedEvent.
        /// </summary>
        public void AddFunds(long amount)
        {
            currentBalance += amount;
            Debug.LogWarning($"Added {amount}. Current Balance: {currentBalance}");

            EventBus<BalanceChangedEvent>.Raise(new BalanceChangedEvent
            {
                UpdatedBalance = currentBalance
            });
        }

        /// <summary>
        /// Returns the current wallet balance.
        /// </summary>
        public long GetBalance()
        {
            return currentBalance;
        }
    }
}
