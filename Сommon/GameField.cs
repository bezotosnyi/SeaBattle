namespace Сommon
{
    using System.Collections.Generic;
    using System.Drawing;

    /// <summary>
    /// Игровое поле
    /// </summary>
    public class GameField
    {
        /// <summary>
        /// Установленные корабли
        /// </summary>
        public readonly PastedShip PastedShip = new PastedShip();

        /// <summary>
        /// Ходы
        /// </summary>
        private readonly List<Move> moves = new List<Move>();

        /// <summary>
        /// Ходы
        /// </summary>
        private readonly bool[,] gameField = new bool[Constants.Size, Constants.Size];

        /// <summary>
        /// Координаты клеток игрового поля
        /// </summary>
        private readonly ResizableRectangle[,] cellCoordinates = new ResizableRectangle[Constants.Size, Constants.Size];

        /// <summary>
        /// Корабли
        /// </summary>
        private readonly List<Ship> ships = new List<Ship>();

        /// <summary>
        /// Кисть для рисования корабля
        /// </summary>
        private readonly Pen shipPen = new Pen(Color.Black, 5f);

        /// <summary>
        /// Кисть для рисования хода
        /// </summary>
        private readonly Pen movePen = new Pen(Color.Black, 5f);

        /// <summary>
        /// Заливка хода
        /// </summary>
        private readonly Brush moveBrush = new SolidBrush(Color.Black);

        public GameField()
        {
            this.ClearGameField();
        }

        /// <summary>
        /// Установка хода
        /// </summary>
        public bool SetMove(Point index, MoveType moveType)
        {
            // если содержится точка
            if (this.moves.Contains(index))
                return false;

            switch (moveType)
            {
                // промах
                case MoveType.Circle:
                    // создаем ход типа точка
                    var move = new Move(
                        MoveType.Circle,
                        new Circle(
                            new ResizableRectangle(
                                this.cellCoordinates[index.X, index.Y].X + this.cellCoordinates[index.X, index.Y].Width / 2.5f,
                                this.cellCoordinates[index.X, index.Y].Y + this.cellCoordinates[index.X, index.Y].Height / 2.5f,
                                this.cellCoordinates[index.X, index.Y].Width / 5f,
                                this.cellCoordinates[index.X, index.Y].Height / 5f)));
                    move.Index = index;
                    this.moves.Add(move);
                    break;

                // попадание
                case MoveType.Cross:
                    // создаем ход типа крестик
                    var mv = new Move(
                        MoveType.Cross,
                        new Cross(
                            new Line(
                                new PointF(
                                    this.cellCoordinates[index.X, index.Y].X,
                                    this.cellCoordinates[index.X, index.Y].Y),
                                new PointF(
                                    this.cellCoordinates[index.X, index.Y].X
                                        + this.cellCoordinates[index.X, index.Y].Width,
                                    this.cellCoordinates[index.X, index.Y].Y
                                        + this.cellCoordinates[index.X, index.Y].Height)),
                            new Line(
                                new PointF(
                                    this.cellCoordinates[index.X, index.Y].X
                                        + this.cellCoordinates[index.X, index.Y].Width,
                                    this.cellCoordinates[index.X, index.Y].Y),
                                new PointF(this.cellCoordinates[index.X, index.Y].X,
                                    this.cellCoordinates[index.X, index.Y].Y + this.cellCoordinates[index.X, index.Y].Height))));
                    mv.Index = index;
                    this.moves.Add(mv);
                    break;
            }

            return true;
        }

        /// <summary>
        /// Игровое поле
        /// </summary>
        public bool[,] GetGameField() => this.gameField;

        /// <summary>
        /// Получение кораблей
        /// </summary>
        public List<ShipData> GetShips()
        {
            var shipData = new List<ShipData>();
            foreach (var ship in this.ships)
            {
                shipData.Add(new ShipData(ship.PastePoint, ship.ShipType, ship.ShipOrientation));
            }

            return shipData;
        }

        /// <summary>
        /// Расчет клеток
        /// </summary>
        /// <param name="width">Ширина</param>
        /// <param name="height">Высота</param>
        public void CalculationGameField(int width, int height)
        {
            for (int i = 0; i < this.cellCoordinates.GetLength(0); i++)
            {
                for (int j = 0; j < this.cellCoordinates.GetLength(1); j++)
                {
                    this.cellCoordinates[i, j] = new ResizableRectangle(
                        x: (j + 1) * width / (this.gameField.GetLength(1) + 1f), 
                        y: (i + 1) * height / (this.gameField.GetLength(0) + 1f), 
                        width: width / (this.gameField.GetLength(1) + 1f), 
                        height: height / (this.gameField.GetLength(0) + 1f));
                }
            }

            this.shipPen.Width = (width + height) / (Constants.Size * 20f);
            this.movePen.Width = (width + height) / (Constants.Size * 20f);
            this.RecalculateShipPosition();
            this.RecalculateMoves();
        }

        /// <summary>
        /// Вставка корабля
        /// </summary>
        /// <param name="point">Точка клика</param>
        /// <param name="type">Тип корабля</param>
        /// <param name="orientation">Положение корабля</param>
        /// <returns>Можно ли вставить корабль в данную позицию</returns>
        public bool PasteShip(Point point, ShipType type, ShipOrientation orientation)
        {
            var result = this.CheckingHitWithGettingIndex(point);
            var hit = result.Item1;
            var index = result.Item2;

            // если клик не в игровом поле
            if (!hit)
                return false;

            var helper = new ArrangeShipHelper(this.cellCoordinates, this.gameField);

            // возможность вставки
            var can = helper.CanPasteShip(index, type, orientation);

            if (!can)
                return false;

            // добавление корабля
            this.AddShip(index, type, orientation);

            return true;
        }

        /// <summary>
        /// Вставка чужого корбля
        /// </summary>
        public void PasteEnemyShip(Point index, ShipType type, ShipOrientation orientation)
        {
            if (!this.ships.Contains(index))
                this.AddShip(index, type, orientation);

            foreach (var ship in this.ships)
            {
                ship.ShipPoints.Clear();
            }
        }

        /// <summary>
        /// Рисование чужих кораблей
        /// </summary>
        public void DrawEnemyShips(Graphics graphics)
        {
            foreach (var ship in this.ships)
            {
                // если убит то рисуем
                if (ship.IsKilled)
                {
                    graphics.DrawRectangles(this.shipPen, new[] { (RectangleF)ship.ShipRectangle });

                    foreach (var neighborsPoint in ship.NeighborsPoints)
                    {
                        var circle = new Circle(
                            new ResizableRectangle(
                                this.cellCoordinates[neighborsPoint.X, neighborsPoint.Y].X
                                + this.cellCoordinates[neighborsPoint.X, neighborsPoint.Y].Width / 2.5f,
                                this.cellCoordinates[neighborsPoint.X, neighborsPoint.Y].Y
                                + this.cellCoordinates[neighborsPoint.X, neighborsPoint.Y].Height / 2.5f,
                                this.cellCoordinates[neighborsPoint.X, neighborsPoint.Y].Width / 5f,
                                this.cellCoordinates[neighborsPoint.X, neighborsPoint.Y].Height / 5f));

                        graphics.FillEllipse(this.moveBrush, (RectangleF)circle.CircleRect);
                    }
                }
            }
        }

        /// <summary>
        /// Рисование кораблей
        /// </summary>
        /// <param name="graphics">
        /// The graphics.
        /// </param>
        public void DrawShips(Graphics graphics)
        {
            foreach (var ship in this.ships)
            {
                graphics.DrawRectangles(this.shipPen, new[] { (RectangleF)ship.ShipRectangle });
            }
        }

        /// <summary>
        /// Проверка попадания в игровое поле с получением индекса
        /// </summary>
        /// <param name="point">Точка клика</param>
        /// <returns>Попадание и индекс попадания</returns>
        public (bool, Point) CheckingHitWithGettingIndex(Point point)
        {
            var hit = false;
            var index = new Point(-1, -1);

            for (int i = 0; i < this.cellCoordinates.GetLength(0); i++)
            {
                for (int j = 0; j < this.cellCoordinates.GetLength(1); j++)
                {
                    if (((RectangleF)this.cellCoordinates[i, j]).Contains(point))
                    {
                        index.X = i;
                        index.Y = j;
                        hit = true;
                        break;
                    }
                }
            }

            return (hit, index);
        }

        /// <summary>
        /// Рисование ходов
        /// </summary>
        public void DrawMoves(Graphics graphics)
        {
            foreach (var move in this.moves)
            {
                switch (move.MoveType)
                {
                    case MoveType.Cross:
                        graphics.DrawLine(this.movePen, move.Cross.LeftLine.StartPoint, move.Cross.LeftLine.EndPoint);
                        graphics.DrawLine(this.movePen, move.Cross.RightLine.StartPoint, move.Cross.RightLine.EndPoint);
                        break;

                    case MoveType.Circle:
                        graphics.FillEllipse(this.moveBrush, (RectangleF)move.Circle.CircleRect);
                        break;
                }
            }
        }

        /// <summary>
        /// Добавление корабля
        /// </summary>
        private void AddShip(Point index, ShipType type, ShipOrientation orientation)
        {
            switch (type)
            {
                case ShipType.One:
                    this.PastedShip.One++;
                    this.ships.Add(new Ship(this.cellCoordinates[index.X, index.Y], type, orientation, index));
                    this.gameField[index.X, index.Y] = true;
                    break;

                case ShipType.Two:
                    this.AddTwoShip(index, type, orientation);
                    break;

                case ShipType.Three:
                    this.AddThreeShip(index, type, orientation);
                    break;
                case ShipType.Four:
                    this.AddFourShip(index, type, orientation);
                    break;
            }
        }

        // добавление двупалубного корабля
        private void AddTwoShip(Point index, ShipType type, ShipOrientation orientation)
        {
            switch (orientation)
            {
                case ShipOrientation.Vertical:
                    this.PastedShip.Two++;
                    var ship = new ResizableRectangle(
                        this.cellCoordinates[index.X, index.Y].X,
                        this.cellCoordinates[index.X, index.Y].Y,
                        this.cellCoordinates[index.X, index.Y].Width,
                        2 * this.cellCoordinates[index.X, index.Y].Height);
                    this.ships.Add(new Ship(ship, type, orientation, index));
                    this.gameField[index.X, index.Y] = true;
                    this.gameField[index.X + 1, index.Y] = true;
                    break;

                case ShipOrientation.Horizontal:
                    this.PastedShip.Two++;
                    var shp = new ResizableRectangle(
                        this.cellCoordinates[index.X, index.Y].X,
                        this.cellCoordinates[index.X, index.Y].Y,
                        2 * this.cellCoordinates[index.X, index.Y].Width,
                        this.cellCoordinates[index.X, index.Y].Height);
                    this.ships.Add(new Ship(shp, type, orientation, index));
                    this.gameField[index.X, index.Y] = true;
                    this.gameField[index.X, index.Y + 1] = true;
                    break;
            }
        }

        // добавление трехпалубного корабля
        private void AddThreeShip(Point index, ShipType type, ShipOrientation orientation)
        {
            switch (orientation)
            {
                case ShipOrientation.Vertical:
                    this.PastedShip.Three++;
                    var ship = new ResizableRectangle(
                        this.cellCoordinates[index.X, index.Y].X,
                        this.cellCoordinates[index.X, index.Y].Y,
                        this.cellCoordinates[index.X, index.Y].Width,
                        3 * this.cellCoordinates[index.X, index.Y].Height);
                    this.ships.Add(new Ship(ship, type, orientation, index));
                    this.gameField[index.X, index.Y] = true;
                    this.gameField[index.X + 1, index.Y] = true;
                    this.gameField[index.X + 2, index.Y] = true;
                    break;

                case ShipOrientation.Horizontal:
                    this.PastedShip.Three++;
                    var shp = new ResizableRectangle(
                        this.cellCoordinates[index.X, index.Y].X,
                        this.cellCoordinates[index.X, index.Y].Y,
                        3 * this.cellCoordinates[index.X, index.Y].Width,
                        this.cellCoordinates[index.X, index.Y].Height);
                    this.ships.Add(new Ship(shp, type, orientation, index));
                    this.gameField[index.X, index.Y] = true;
                    this.gameField[index.X, index.Y + 1] = true;
                    this.gameField[index.X, index.Y + 2] = true;
                    break;
            }
        }

        // добавление четырехпалубного корабля
        private void AddFourShip(Point index, ShipType type, ShipOrientation orientation)
        {
            switch (orientation)
            {
                case ShipOrientation.Vertical:
                    this.PastedShip.Four++;
                    var ship = new ResizableRectangle(
                        this.cellCoordinates[index.X, index.Y].X,
                        this.cellCoordinates[index.X, index.Y].Y,
                        this.cellCoordinates[index.X, index.Y].Width,
                        4 * this.cellCoordinates[index.X, index.Y].Height);
                    this.ships.Add(new Ship(ship, type, orientation, index));
                    this.gameField[index.X, index.Y] = true;
                    this.gameField[index.X + 1, index.Y] = true;
                    this.gameField[index.X + 2, index.Y] = true;
                    this.gameField[index.X + 3, index.Y] = true;
                    break;

                case ShipOrientation.Horizontal:
                    this.PastedShip.Four++;
                    var shp = new ResizableRectangle(
                        this.cellCoordinates[index.X, index.Y].X,
                        this.cellCoordinates[index.X, index.Y].Y,
                        4 * this.cellCoordinates[index.X, index.Y].Width,
                        this.cellCoordinates[index.X, index.Y].Height);
                    this.ships.Add(new Ship(shp, type, orientation, index));
                    this.gameField[index.X, index.Y] = true;
                    this.gameField[index.X, index.Y + 1] = true;
                    this.gameField[index.X, index.Y + 2] = true;
                    this.gameField[index.X, index.Y + 3] = true;
                    break;
            }
        }

        /// <summary>
        /// Перерасчет позиций кораблей
        /// </summary>
        private void RecalculateShipPosition()
        {
            foreach (var ship in this.ships)
            {
                switch (ship.ShipType)
                {
                    case ShipType.One:
                        ship.ShipRectangle = this.cellCoordinates[ship.PastePoint.X, ship.PastePoint.Y];
                        break;

                    case ShipType.Two:
                        this.RecalculateTwoShipPosition(ship);
                        break;

                    case ShipType.Three:
                        this.RecalculateThreeShipPosition(ship);
                        break;
                    case ShipType.Four:
                        this.RecalculateFourShipPosition(ship);
                        break;
                }
            }
        }

        // перерасчет двупалубного корабля
        private void RecalculateTwoShipPosition(Ship ship)
        {
            switch (ship.ShipOrientation)
            {
                case ShipOrientation.Vertical:
                    if (ship.PastePoint.X == this.gameField.GetLength(0) - 1)
                    {
                        var shp = new ResizableRectangle(
                            this.cellCoordinates[ship.PastePoint.X - 1, ship.PastePoint.Y].X,
                            this.cellCoordinates[ship.PastePoint.X - 1, ship.PastePoint.Y].Y,
                            this.cellCoordinates[ship.PastePoint.X, ship.PastePoint.Y].Width,
                            2 * this.cellCoordinates[ship.PastePoint.X, ship.PastePoint.Y].Height);
                        ship.ShipRectangle = shp;
                    }
                    else
                    {
                        var shp = new ResizableRectangle(
                            this.cellCoordinates[ship.PastePoint.X, ship.PastePoint.Y].X,
                            this.cellCoordinates[ship.PastePoint.X, ship.PastePoint.Y].Y,
                            this.cellCoordinates[ship.PastePoint.X, ship.PastePoint.Y].Width,
                            2 * this.cellCoordinates[ship.PastePoint.X, ship.PastePoint.Y].Height);
                        ship.ShipRectangle = shp;
                    }

                    break;

                case ShipOrientation.Horizontal:
                    if (ship.PastePoint.Y == this.gameField.GetLength(0) - 1)
                    {
                        var shp = new ResizableRectangle(
                            this.cellCoordinates[ship.PastePoint.X, ship.PastePoint.Y - 1].X,
                            this.cellCoordinates[ship.PastePoint.X, ship.PastePoint.Y - 1].Y,
                            2 * this.cellCoordinates[ship.PastePoint.X, ship.PastePoint.Y].Width,
                            this.cellCoordinates[ship.PastePoint.X, ship.PastePoint.Y].Height);
                        ship.ShipRectangle = shp;
                    }
                    else
                    {
                        var shp = new ResizableRectangle(
                            this.cellCoordinates[ship.PastePoint.X, ship.PastePoint.Y].X,
                            this.cellCoordinates[ship.PastePoint.X, ship.PastePoint.Y].Y,
                            2 * this.cellCoordinates[ship.PastePoint.X, ship.PastePoint.Y].Width,
                            this.cellCoordinates[ship.PastePoint.X, ship.PastePoint.Y].Height);
                        ship.ShipRectangle = shp;
                    }

                    break;
            }
        }

        // перерасчет трехпалубного корабля
        private void RecalculateThreeShipPosition(Ship ship)
        {
            switch (ship.ShipOrientation)
            {
                case ShipOrientation.Vertical:
                    if (ship.PastePoint.X == this.gameField.GetLength(0) - 1)
                    {
                        var shp = new ResizableRectangle(
                            this.cellCoordinates[ship.PastePoint.X - 2, ship.PastePoint.Y].X,
                            this.cellCoordinates[ship.PastePoint.X - 2, ship.PastePoint.Y].Y,
                            this.cellCoordinates[ship.PastePoint.X, ship.PastePoint.Y].Width,
                            3 * this.cellCoordinates[ship.PastePoint.X, ship.PastePoint.Y].Height);
                        ship.ShipRectangle = shp;
                    }
                    else
                    {
                        var shp = new ResizableRectangle(
                            this.cellCoordinates[ship.PastePoint.X, ship.PastePoint.Y].X,
                            this.cellCoordinates[ship.PastePoint.X, ship.PastePoint.Y].Y,
                            this.cellCoordinates[ship.PastePoint.X, ship.PastePoint.Y].Width,
                            3 * this.cellCoordinates[ship.PastePoint.X, ship.PastePoint.Y].Height);
                        ship.ShipRectangle = shp;
                    }

                    break;

                case ShipOrientation.Horizontal:
                    if (ship.PastePoint.Y == this.gameField.GetLength(0) - 1)
                    {
                        var shp = new ResizableRectangle(
                            this.cellCoordinates[ship.PastePoint.X, ship.PastePoint.Y - 2].X,
                            this.cellCoordinates[ship.PastePoint.X, ship.PastePoint.Y - 2].Y,
                            3 * this.cellCoordinates[ship.PastePoint.X, ship.PastePoint.Y].Width,
                            this.cellCoordinates[ship.PastePoint.X, ship.PastePoint.Y].Height);
                        ship.ShipRectangle = shp;
                    }
                    else
                    {
                        var shp = new ResizableRectangle(
                            this.cellCoordinates[ship.PastePoint.X, ship.PastePoint.Y].X,
                            this.cellCoordinates[ship.PastePoint.X, ship.PastePoint.Y].Y,
                            3 * this.cellCoordinates[ship.PastePoint.X, ship.PastePoint.Y].Width,
                            this.cellCoordinates[ship.PastePoint.X, ship.PastePoint.Y].Height);
                        ship.ShipRectangle = shp;
                    }

                    break;
            }
        }

        // перерасчет четырехпалубного корабля
        private void RecalculateFourShipPosition(Ship ship)
        {
            switch (ship.ShipOrientation)
            {
                case ShipOrientation.Vertical:
                    if (ship.PastePoint.X == this.gameField.GetLength(0) - 1)
                    {
                        var shp = new ResizableRectangle(
                            this.cellCoordinates[ship.PastePoint.X - 3, ship.PastePoint.Y].X,
                            this.cellCoordinates[ship.PastePoint.X - 3, ship.PastePoint.Y].Y,
                            this.cellCoordinates[ship.PastePoint.X, ship.PastePoint.Y].Width,
                            4 * this.cellCoordinates[ship.PastePoint.X, ship.PastePoint.Y].Height);
                        ship.ShipRectangle = shp;
                    }
                    else
                    {
                        var shp = new ResizableRectangle(
                            this.cellCoordinates[ship.PastePoint.X, ship.PastePoint.Y].X,
                            this.cellCoordinates[ship.PastePoint.X, ship.PastePoint.Y].Y,
                            this.cellCoordinates[ship.PastePoint.X, ship.PastePoint.Y].Width,
                            4 * this.cellCoordinates[ship.PastePoint.X, ship.PastePoint.Y].Height);
                        ship.ShipRectangle = shp;
                    }

                    break;

                case ShipOrientation.Horizontal:
                    if (ship.PastePoint.Y == this.gameField.GetLength(0) - 1)
                    {
                        var shp = new ResizableRectangle(
                            this.cellCoordinates[ship.PastePoint.X, ship.PastePoint.Y - 3].X,
                            this.cellCoordinates[ship.PastePoint.X, ship.PastePoint.Y - 3].Y,
                            4 * this.cellCoordinates[ship.PastePoint.X, ship.PastePoint.Y].Width,
                            this.cellCoordinates[ship.PastePoint.X, ship.PastePoint.Y].Height);
                        ship.ShipRectangle = shp;
                    }
                    else
                    {
                        var shp = new ResizableRectangle(
                            this.cellCoordinates[ship.PastePoint.X, ship.PastePoint.Y].X,
                            this.cellCoordinates[ship.PastePoint.X, ship.PastePoint.Y].Y,
                            4 * this.cellCoordinates[ship.PastePoint.X, ship.PastePoint.Y].Width,
                            this.cellCoordinates[ship.PastePoint.X, ship.PastePoint.Y].Height);
                        ship.ShipRectangle = shp;
                    }

                    break;
            }
        }

        /// <summary>
        /// Очистка игрового поля
        /// </summary>
        private void ClearGameField()
        {
            for (int i = 0; i < this.gameField.GetLength(0); i++)
            {
                for (int j = 0; j < this.gameField.GetLength(1); j++)
                {
                    this.gameField[i, j] = false;
                }
            }
        }

        // перерасчет ходов
        private void RecalculateMoves()
        {
            foreach (var move in this.moves)
            {
                Point index = move.Index;
                switch (move.MoveType)
                {
                    case MoveType.Circle:
                        var circle = new Circle(
                            new ResizableRectangle(
                                this.cellCoordinates[index.X, index.Y].X + this.cellCoordinates[index.X, index.Y].Width / 2.5f,
                                this.cellCoordinates[index.X, index.Y].Y + this.cellCoordinates[index.X, index.Y].Height / 2.5f,
                                this.cellCoordinates[index.X, index.Y].Width / 5f,
                                this.cellCoordinates[index.X, index.Y].Height / 5f));
                        move.Circle = circle;
                        break;

                    case MoveType.Cross:
                        var cross = new Cross(
                            new Line(
                                new PointF(
                                    this.cellCoordinates[index.X, index.Y].X,
                                    this.cellCoordinates[index.X, index.Y].Y),
                                new PointF(
                                    this.cellCoordinates[index.X, index.Y].X
                                    + this.cellCoordinates[index.X, index.Y].Width,
                                    this.cellCoordinates[index.X, index.Y].Y
                                    + this.cellCoordinates[index.X, index.Y].Height)),
                            new Line(
                                new PointF(
                                    this.cellCoordinates[index.X, index.Y].X
                                    + this.cellCoordinates[index.X, index.Y].Width,
                                    this.cellCoordinates[index.X, index.Y].Y),
                                new PointF(
                                    this.cellCoordinates[index.X, index.Y].X,
                                    this.cellCoordinates[index.X, index.Y].Y
                                    + this.cellCoordinates[index.X, index.Y].Height)));

                        move.Cross = cross;
                        break;
                }
            }
        }
    }
}