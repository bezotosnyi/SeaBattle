namespace Сommon
{
    using System;
    using System.Drawing;

    /// <summary>
    /// Ход для передачи между каналами
    /// </summary>
    [Serializable]
    public class MoveData
    {
        /// <summary>
        /// Точка
        /// </summary>
        public Point Point { get; set; }

        /// <summary>
        /// Тип хода
        /// </summary>
        public MoveType MoveType { get; set; }

        public MoveData(Point point, MoveType moveType)
        {
            this.Point = point;
            this.MoveType = moveType;
        }
    }
}