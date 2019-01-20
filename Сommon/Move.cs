namespace Сommon
{
    using System.Drawing;

    /// <summary>
    /// Ход
    /// </summary>
    public class Move
    {
        /// <summary>
        /// Индекс хода
        /// </summary>
        public Point Index { get; set; }

        /// <summary>
        /// Тип хода
        /// </summary>
        public MoveType MoveType { get; set; }

        /// <summary>
        /// Круг
        /// </summary>
        public Circle Circle { get; set; }

        /// <summary>
        /// Крестик
        /// </summary>
        public Cross Cross { get; set; }

        public Move(MoveType moveType, Circle circle)
        {
            this.MoveType = moveType;
            this.Circle = circle;
        }

        public Move(MoveType moveType, Cross cross)
        {
            this.MoveType = moveType;
            this.Cross = cross;
        }
    }
}