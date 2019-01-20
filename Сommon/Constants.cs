namespace Сommon
{
    /// <summary>
    /// Константы
    /// </summary>
    public class Constants
    {
        /// <summary>
        /// Размер игрового поля
        /// </summary>
        public const int Size = 10;

        /// <summary>
        /// Количество однопалубных
        /// </summary>
        public const int MaxOne = 4;

        /// <summary>
        /// Количество двухпалубных
        /// </summary>
        public const int MaxTwo = 3;

        /// <summary>
        /// Количество трехпалубных
        /// </summary>
        public const int MaxThree = 2;

        /// <summary>
        /// Количество четырехпалубных
        /// </summary>
        public const int MaxFour = 1;

        public static int ShipCount() => MaxOne + MaxTwo + MaxThree + MaxFour;
    }
}