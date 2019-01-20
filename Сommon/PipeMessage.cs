namespace Сommon
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;

    /// <summary>
    /// Сообщение канала
    /// </summary>
    [Serializable]
    public class PipeMessage
    {
        /// <summary>
        /// Тип сообщения
        /// </summary>
        public PipeMessageType PipeMessageType { get; set; }

        /// <summary>
        /// Id игрока
        /// </summary>
        public int PlayerId { get; set; }

        /// <summary>
        /// Игровое поле
        /// </summary>
        public bool[,] GameField { get; set; }

        /// <summary>
        /// Индекс
        /// </summary>
        public Point Index { get; set; }

        /// <summary>
        /// Список ходов
        /// </summary>
        public List<MoveData> MoveData { get; set; }

        /// <summary>
        /// Список кораблей
        /// </summary>
        public List<ShipData> ShipData { get; set; }

        /// <summary>
        /// Тип хода
        /// </summary>
        public MoveType MoveType { get; set; }

        /// <summary>
        /// Сообщение
        /// </summary>
        public string Message { get; set; }
    }
}