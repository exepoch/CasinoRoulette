namespace User
{
    /// <summary>
    /// Interface representing wallet operations.
    /// Abstracts wallet logic to allow different implementations and easier testing.
    /// </summary>
    public interface IWalletService
    {
        /// <summary>
        /// Attempts to spend the specified amount from the wallet.
        /// Returns true if spending was successful; false if insufficient balance.
        /// </summary>
        bool TrySpend(long amount);

        /// <summary>
        /// Adds the specified amount of funds to the wallet.
        /// </summary>
        void AddFunds(long amount);

        /// <summary>
        /// Gets the current wallet balance.
        /// </summary>
        long GetBalance();
    }
}