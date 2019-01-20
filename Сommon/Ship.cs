namespace Сommon
{
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;

    /// <summary>
    /// Корабль
    /// </summary>
    public class Ship
    {
        /// <summary>
        /// Прямоугольник, которые представляет корабль
        /// </summary>
        public ResizableRectangle ShipRectangle { get; set; }

        /// <summary>
        /// Тип корабля
        /// </summary>
        public ShipType ShipType { get; set; }

        /// <summary>
        /// Тип вставки
        /// </summary>
        public ShipOrientation ShipOrientation { get; set; }

        /// <summary>
        /// Позиция вставки
        /// </summary>
        public Point PastePoint { get; set; }

        /// <summary>
        /// Убит ли корбль (нет точек корабля)
        /// </summary>
        public bool IsKilled => this.ShipPoints.Count == 0;

        /// <summary>
        /// Точки корбля
        /// </summary>
        public List<Point> ShipPoints { get; private set; } = new List<Point>();

        /// <summary>
        /// Соседние точки
        /// </summary>
        public List<Point> NeighborsPoints { get; private set; } = new List<Point>();

        public Ship(ResizableRectangle shipRectangle, ShipType shipType, ShipOrientation shipOrientation, Point pastePoint)
        {
            this.ShipRectangle = shipRectangle;
            this.ShipType = shipType;
            this.ShipOrientation = shipOrientation;
            this.PastePoint = pastePoint;

            this.CalculateShipPoints();
            this.CalculateNeighborsPoints();
        }

        public Ship(Point pastePoint, ShipType shipType, ShipOrientation shipOrientation)
        {
            this.PastePoint = pastePoint;
            this.ShipType = shipType;
            this.ShipOrientation = shipOrientation;

            this.CalculateShipPoints();
            this.CalculateNeighborsPoints();
        }

        /// <summary>
        /// Расчет точек корабля
        /// </summary>
        private void CalculateShipPoints()
        {
            switch (this.ShipOrientation)
            {
                case ShipOrientation.Vertical:
                    for (int i = 0; i < this.ShipType.ToInt(); i++)
                        this.ShipPoints.Add(new Point(this.PastePoint.X + i, this.PastePoint.Y));
                    break;

                case ShipOrientation.Horizontal:
                    for (int i = 0; i < this.ShipType.ToInt(); i++)
                        this.ShipPoints.Add(new Point(this.PastePoint.X, this.PastePoint.Y + i));
                    break;
            }
        }

        /// <summary>
        /// Расчет соседних точек
        /// </summary>
        private void CalculateNeighborsPoints()
        {
            switch (this.ShipOrientation)
            {
                case ShipOrientation.Vertical:
                    for (int i = -1; i <= this.ShipType.ToInt(); i++)
                    {
                        this.NeighborsPoints.Add(new Point(this.PastePoint.X + i, this.PastePoint.Y - 1));
                        this.NeighborsPoints.Add(new Point(this.PastePoint.X + i, this.PastePoint.Y + 1));
                    }

                    this.NeighborsPoints.Add(new Point(this.PastePoint.X - 1, this.PastePoint.Y));
                    this.NeighborsPoints.Add(new Point(this.PastePoint.X + this.ShipType.ToInt(), this.PastePoint.Y));
                    break;

                case ShipOrientation.Horizontal:
                    for (int i = -1; i <= this.ShipType.ToInt(); i++)
                    {
                        this.NeighborsPoints.Add(new Point(this.PastePoint.X - 1, this.PastePoint.Y + i));
                        this.NeighborsPoints.Add(new Point(this.PastePoint.X + 1, this.PastePoint.Y + i));
                    }

                    this.NeighborsPoints.Add(new Point(this.PastePoint.X, this.PastePoint.Y - 1));
                    this.NeighborsPoints.Add(new Point(this.PastePoint.X, this.PastePoint.Y + this.ShipType.ToInt()));

                    break;
            }

            this.NormalizeNeighborsPoints();
        }

        /// <summary>
        /// Нормализация соседних точек (удаление отрицательных)
        /// </summary>
        private void NormalizeNeighborsPoints()
        {
            foreach (var point in this.NeighborsPoints.ToList())
            {
                if (point.X < 0 || point.Y < 0 || point.X > Constants.Size - 1 || point.Y > Constants.Size - 1)
                {
                    this.NeighborsPoints.Remove(point);
                }
            }
        }
    }
}