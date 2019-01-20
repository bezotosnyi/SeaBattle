namespace Сommon
{
    using System;
    using System.Drawing;

    /// <summary>
    /// Данные корбля для передачи в канале
    /// </summary>
    [Serializable]
    public class ShipData
    {        
        /// <summary>
        /// Позиция вставки
        /// </summary>
        public Point PastePoint { get; set; }

        /// <summary>
        /// Тип корабля
        /// </summary>
        public ShipType ShipType { get; set; }

        /// <summary>
        /// Тип вставки
        /// </summary>
        public ShipOrientation ShipOrientation { get; set; }

        public ShipData(Point pastePoint, ShipType shipType, ShipOrientation shipOrientation)
        {
            this.PastePoint = pastePoint;
            this.ShipType = shipType;
            this.ShipOrientation = shipOrientation;
        }
    }
}