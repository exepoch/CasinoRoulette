namespace Gameplay.Betting.Data
{
    /// <summary>
    /// Enumeration representing all supported types of bets in the roulette game.
    /// Covers both Inside Bets (e.g., Straight, Split) and Outside Bets (e.g., Red, Even),
    /// as well as special zero bets for European and American Roulette variants.
    /// </summary>
    public enum BetType
    {
        Straight,
        Split,
        Street,
        Corner,
        SixLine,
        Column,
        Dozen,
        Red,
        Black,
        Even,
        Odd,
        Low,
        High,
        Zero,
        DoubleZero // Enabled only for American Roulette variant
    }
}