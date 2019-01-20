namespace Сommon
{
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;

    /// <summary>
    /// Вспомогательные методы расширений
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Перевод типа корабля в число
        /// </summary>
        public static int ToInt(this ShipType shipType)
        {
            switch (shipType)
            {
                case ShipType.None:
                    return 0;
                case ShipType.One:
                    return 1;
                case ShipType.Two:
                    return 2;
                case ShipType.Three:
                    return 3;
                case ShipType.Four:
                    return 4;
            }

            return -1;
        }

        /// <summary>
        /// Содержат ли движения точку
        /// </summary>
        public static bool Contains(this IEnumerable<Move> moves, Point point)
        {
            return moves.Any(x => x.Index == point);
        }

        /// <summary>
        /// Содержат ли движения точку
        /// </summary>
        public static bool Contains(this IEnumerable<MoveData> moves, Point point)
        {
            return moves.Any(x => x.Point == point);
        }

        /// <summary>
        /// Содержат ли корабли точку
        /// </summary>
        public static bool Contains(this IEnumerable<ShipData> moves, Point point)
        {
            return moves.Any(x => x.PastePoint == point);
        }

        /// <summary>
        /// Перевод из ShipData в Ship
        /// </summary>
        public static IEnumerable<Ship> ToShipList(this IEnumerable<ShipData> ships)
        {
            return ships.Select(ship => new Ship(ship.PastePoint, ship.ShipType, ship.ShipOrientation));
        }

        /// <summary>
        /// Перевод из Ship в ShipData
        /// </summary>
        public static IEnumerable<ShipData> ToShipDataList(this IEnumerable<Ship> ships)
        {
            return ships.Select(ship => new ShipData(ship.PastePoint, ship.ShipType, ship.ShipOrientation));
        }

        /// <summary>
        /// Содержит ли корабль точку
        /// </summary>
        public static bool Contains(this IEnumerable<Ship> moves, Point point)
        {
            return moves.Any(x => x.PastePoint == point);
        }

        /// <summary>
        /// Перевод типа корбля в строковое представление
        /// </summary>
        public static string ToStringName(this ShipType shipType)
        {
            switch (shipType)
            {
                case ShipType.One:
                    return "Однопалубник";
                case ShipType.Two:
                    return "Двопалубник";
                case ShipType.Three:
                    return "Трехпалубник";
                case ShipType.Four:
                    return "Четырехпалубник";
            }

            return string.Empty;
        }

        /// <summary>
        /// Тип движения в строковое представление
        /// </summary>
        public static string ToStringName(this MoveType moveType)
        {
            switch (moveType)
            {
                case MoveType.Cross:
                    return "Попадание";
                case MoveType.Circle:
                    return "Промах";
            }

            return string.Empty;
        }
    }
}