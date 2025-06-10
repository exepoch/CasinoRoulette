using System.Linq;

namespace Utils
{
    public static class Utils
    {
        public static string NumberToName(this int number)
        {
            if (number is >= 0 and < 37)
            {
                return $"{number}";
            }

            return number == 37 ? "00" : "Invalid Number";
        }

        private static readonly int[] EuropeanNumbers = {
            0, 26, 3, 35, 12, 28, 7, 29, 18, 22, 9, 31, 14, 20,
            1, 33, 16, 24, 5, 10, 23, 8, 30, 11, 36, 13, 27, 6,
            34, 17, 25, 2, 21, 4, 19, 15, 32
        };

        private static readonly int[] AmericanNumbers = EuropeanNumbers.Concat(new[] { 37 }).ToArray(); // 37 represents "00"

        public static int[] RouletTypeToNumbers(this RouletteType type)
        {
            return type == RouletteType.European
                ? EuropeanNumbers
                : AmericanNumbers;
        }

        public static int ToNumberCount(this RouletteType type) => type == RouletteType.European ? 37 : 38;
    }
}
