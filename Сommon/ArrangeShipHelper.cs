namespace Сommon
{
    using System.Drawing;

    /// <summary>
    /// Помощник в расстановке кораблей
    /// </summary>
    public class ArrangeShipHelper
    {
        /// <summary>
        /// Координаты клеток игрового поля
        /// </summary>
        private readonly ResizableRectangle[,] cellCoordinates;

        /// <summary>
        /// Ходы
        /// </summary>
        private readonly bool[,] gameField;

        public ArrangeShipHelper(ResizableRectangle[,] cellCoordinates, bool[,] gameField)
        {
            this.cellCoordinates = cellCoordinates;
            this.gameField = gameField;
        }

        /// <summary>
        /// Проверка возможности вставки корабля
        /// </summary>
        /// <param name="index">Позиция вставки</param>
        /// <param name="shipType">Тип корабля</param>
        /// <param name="shipOrientation">Положение корабля</param>
        /// <returns>Возможность вставки</returns>
        public bool CanPasteShip(Point index, ShipType shipType, ShipOrientation shipOrientation)
        {
            var canPaste = false;

            // если нельзя вставить
            if (!this.CanPastePositionShip(index, shipType, shipOrientation))
                return false;

            // получаем позицию вставки (см. перечисление PastingShipPosition)
            var pastingPosition = this.GetPastingShipPosition(index, shipType, shipOrientation);

            // проверяем возможность вставки согласно позиции
            switch (pastingPosition)
            {
                case PastingShipPosition.TopLeft:
                    canPaste = this.FreeRight(index, shipType, shipOrientation)
                               && this.FreeBottom(index, shipType, shipOrientation)
                               && this.FreeBottomRight(index, shipType, shipOrientation);

                    break;

                case PastingShipPosition.TopRight:
                    canPaste = this.FreeLeft(index, shipType, shipOrientation)
                               && this.FreeBottomLeft(index, shipType, shipOrientation)
                               && this.FreeBottom(index, shipType, shipOrientation);
                    break;

                case PastingShipPosition.TopOther:
                    canPaste = this.FreeLeft(index, shipType, shipOrientation)
                               && this.FreeRight(index, shipType, shipOrientation)
                               && this.FreeBottom(index, shipType, shipOrientation)
                               && this.FreeBottomLeft(index, shipType, shipOrientation)
                               && this.FreeBottomRight(index, shipType, shipOrientation);
                    break;

                case PastingShipPosition.BottomLeft:
                    canPaste = this.FreeRight(index, shipType, shipOrientation)
                               && this.FreeTop(index, shipType, shipOrientation)
                               && this.FreeTopRight(index, shipType, shipOrientation);
                    break;

                case PastingShipPosition.BottomRight:
                    canPaste = this.FreeLeft(index, shipType, shipOrientation)
                               && this.FreeTop(index, shipType, shipOrientation)
                               && this.FreeTopLeft(index, shipType, shipOrientation);
                    break;

                case PastingShipPosition.BottomOther:
                    canPaste = this.FreeLeft(index, shipType, shipOrientation)
                               && this.FreeRight(index, shipType, shipOrientation)
                               && this.FreeTop(index, shipType, shipOrientation)
                               && this.FreeTopLeft(index, shipType, shipOrientation)
                               && this.FreeTopRight(index, shipType, shipOrientation);
                    break;

                case PastingShipPosition.Left:
                    canPaste = this.FreeTop(index, shipType, shipOrientation)
                               && this.FreeBottom(index, shipType, shipOrientation)
                               && this.FreeRight(index, shipType, shipOrientation)
                               && this.FreeTopRight(index, shipType, shipOrientation)
                               && this.FreeBottomRight(index, shipType, shipOrientation);
                    break;

                case PastingShipPosition.Right:
                    canPaste = this.FreeTop(index, shipType, shipOrientation)
                               && this.FreeBottom(index, shipType, shipOrientation)
                               && this.FreeLeft(index, shipType, shipOrientation)
                               && this.FreeTopLeft(index, shipType, shipOrientation)
                               && this.FreeBottomLeft(index, shipType, shipOrientation);
                    break;

                case PastingShipPosition.Other:
                    canPaste = this.FreeTop(index, shipType, shipOrientation)
                               && this.FreeBottom(index, shipType, shipOrientation)
                               && this.FreeLeft(index, shipType, shipOrientation)
                               && this.FreeRight(index, shipType, shipOrientation)
                               && this.FreeLeft(index, shipType, shipOrientation)
                               && this.FreeBottomLeft(index, shipType, shipOrientation)
                               && this.FreeTopLeft(index, shipType, shipOrientation)
                               && this.FreeTopRight(index, shipType, shipOrientation)
                               && this.FreeBottomRight(index, shipType, shipOrientation);
                    break;
            }

            return canPaste;
        }

        /// <summary>
        /// Проверка вставки корабля
        /// </summary>
        private bool CanPastePositionShip(Point index, ShipType shipType, ShipOrientation shipOrientation)
        {
            switch (shipOrientation)
            {
                case ShipOrientation.Vertical:
                    if (index.X > this.gameField.GetLength(0) - shipType.ToInt())
                        return false;
                    break;
                    
                case ShipOrientation.Horizontal:
                    if (index.Y > this.gameField.GetLength(1) - shipType.ToInt())
                        return false;
                    break;
            }

            return true;
        }

        /*
               left_top ___top__ right_top
                       |        |
                  left |  SHIP  | right
                       |________|
            left_bottom  bottom  rigth_bottom
         */

        /// <summary>
        /// Получение позиции вставки корабля
        /// </summary>
        private PastingShipPosition GetPastingShipPosition(Point index, ShipType shipType, ShipOrientation shipOrientation)
        {
            // верх
            if (index.X == 0)
            {
                if (index.Y == 0)
                    return PastingShipPosition.TopLeft;

                if (shipOrientation == ShipOrientation.Horizontal 
                    && index.Y >= this.cellCoordinates.GetLength(1) - shipType.ToInt())
                    return PastingShipPosition.TopRight;

                if (shipOrientation == ShipOrientation.Vertical
                    && index.Y >= this.cellCoordinates.GetLength(1) - 1)
                    return PastingShipPosition.TopRight;

                return PastingShipPosition.TopOther;
            }

            // низ
            if (index.X == this.cellCoordinates.GetLength(0) - 1)
            {
                if (index.Y == 0)
                    return PastingShipPosition.BottomLeft;

                if (shipOrientation == ShipOrientation.Horizontal
                    && index.Y >= this.cellCoordinates.GetLength(1) - shipType.ToInt())
                    return PastingShipPosition.BottomRight;

                return PastingShipPosition.BottomOther;
            }

            // низ вертикаль
            if (shipOrientation == ShipOrientation.Vertical
                && index.X >= this.cellCoordinates.GetLength(0) - shipType.ToInt())
                return PastingShipPosition.BottomOther;

            // лево
            if (index.Y == 0)
                return PastingShipPosition.Left;

            // право
            if (shipOrientation == ShipOrientation.Horizontal
                && index.Y >= this.cellCoordinates.GetLength(1) - shipType.ToInt())
                return PastingShipPosition.Right;

            if (shipOrientation == ShipOrientation.Vertical
                && index.Y >= this.cellCoordinates.GetLength(1) - 1)
                return PastingShipPosition.Right;

            return PastingShipPosition.Other;
        }

        /// <summary>
        /// Проверка свободно ли сверху
        /// </summary>
        /// <param name="index">Индекс</param>
        /// <param name="shipType">Тип корабля</param>
        /// <param name="shipOrientation">Положение корабля</param>
        /// <returns>Свободно ли сверху</returns>
        private bool FreeTop(Point index, ShipType shipType, ShipOrientation shipOrientation)
        {
            var free = true;

            switch (shipOrientation)
            {
                case ShipOrientation.Vertical:
                    free = this.IsFree(index.X - 1, index.Y);
                    break;

                case ShipOrientation.Horizontal:
                    if (shipType.HasFlag(ShipType.One))
                        free &= this.IsFree(index.X - 1, index.Y);

                    if (shipType.HasFlag(ShipType.Two))
                        free &= this.IsFree(index.X - 1, index.Y + 1);

                    if (shipType.HasFlag(ShipType.Three))
                        free &= this.IsFree(index.X - 1, index.Y + 2);

                    if (shipType.HasFlag(ShipType.Four))
                        free &= this.IsFree(index.X - 1, index.Y + 3);
                    break;
            }

            return free;
        }

        /// <summary>
        /// Проверка свободно ли снизу
        /// </summary>
        /// <param name="index">Индекс</param>
        /// <param name="shipType">Тип корабля</param>
        /// <param name="shipOrientation">Положение корабля</param>
        /// <returns>Свободно ли снизу</returns>
        private bool FreeBottom(Point index, ShipType shipType, ShipOrientation shipOrientation)
        {
            var free = true;

            switch (shipOrientation)
            {
                case ShipOrientation.Vertical:
                    if (shipType.HasFlag(ShipType.One))
                        free &= this.IsFree(index.X + 1, index.Y);

                    if (shipType.HasFlag(ShipType.Two))
                        free &= this.IsFree(index.X + 2, index.Y);

                    if (shipType.HasFlag(ShipType.Three))
                        free &= this.IsFree(index.X + 3, index.Y);

                    if (shipType.HasFlag(ShipType.Four))
                        free &= this.IsFree(index.X + 4, index.Y);
                    break;

                case ShipOrientation.Horizontal:
                    if (shipType.HasFlag(ShipType.One))
                        free &= this.IsFree(index.X + 1, index.Y);

                    if (shipType.HasFlag(ShipType.Two))
                        free &= this.IsFree(index.X + 1, index.Y + 1);

                    if (shipType.HasFlag(ShipType.Three))
                        free &= this.IsFree(index.X + 1, index.Y + 2);

                    if (shipType.HasFlag(ShipType.Four))
                        free &= this.IsFree(index.X + 1, index.Y + 3);
                    break;
            }

            return free;
        }

        /// <summary>
        /// Проверка свободно ли слева
        /// </summary>
        /// <param name="index">Индекс</param>
        /// <param name="shipType">Тип корабля</param>
        /// <param name="shipOrientation">Положение корабля</param>
        /// <returns>Свободно ли слева</returns>
        private bool FreeLeft(Point index, ShipType shipType, ShipOrientation shipOrientation)
        {
            var free = true;

            switch (shipOrientation)
            {
                case ShipOrientation.Vertical:
                    if (shipType.HasFlag(ShipType.One))
                        free &= this.IsFree(index.X, index.Y - 1);

                    if (shipType.HasFlag(ShipType.Two))
                        free &= this.IsFree(index.X + 1, index.Y - 1);

                    if (shipType.HasFlag(ShipType.Three))
                        free &= this.IsFree(index.X + 2, index.Y - 1);

                    if (shipType.HasFlag(ShipType.Four))
                        free &= this.IsFree(index.X + 3, index.Y - 1);
                    break;

                case ShipOrientation.Horizontal:
                    free = this.IsFree(index.X, index.Y - 1);
                    break;
            }

            return free;
        }

        /// <summary>
        /// Проверка свободно ли справа
        /// </summary>
        /// <param name="index">Индекс</param>
        /// <param name="shipType">Тип корабля</param>
        /// <param name="shipOrientation">Положение корабля</param>
        /// <returns>Свободно ли справа</returns>
        private bool FreeRight(Point index, ShipType shipType, ShipOrientation shipOrientation)
        {
            var free = true;

            switch (shipOrientation)
            {
                case ShipOrientation.Vertical:
                    if (shipType.HasFlag(ShipType.One))
                        free &= this.IsFree(index.X, index.Y + 1);

                    if (shipType.HasFlag(ShipType.Two))
                        free &= this.IsFree(index.X + 1, index.Y + 1);

                    if (shipType.HasFlag(ShipType.Three))
                        free &= this.IsFree(index.X + 2, index.Y + 1);

                    if (shipType.HasFlag(ShipType.Four))
                        free &= this.IsFree(index.X + 3, index.Y + 1);
                    break;

                case ShipOrientation.Horizontal:
                    free = this.IsFree(index.X, index.Y + shipType.ToInt());
                    break;
            }

            return free;
        }

        /// <summary>
        /// Проверка свободно ли верх лево
        /// </summary>
        /// <param name="index">Индекс</param>
        /// <param name="shipType">Тип корабля</param>
        /// <param name="shipOrientation">Положение корабля</param>
        /// <returns>Свободно ли верх лево</returns>
        private bool FreeTopLeft(Point index, ShipType shipType, ShipOrientation shipOrientation)
        {
            return this.IsFree(index.X - 1, index.Y - 1);
        }

        /// <summary>
        /// Проверка свободно ли верх право
        /// </summary>
        /// <param name="index">Индекс</param>
        /// <param name="shipType">Тип корабля</param>
        /// <param name="shipOrientation">Положение корабля</param>
        /// <returns>Свободно ли верх право</returns>
        private bool FreeTopRight(Point index, ShipType shipType, ShipOrientation shipOrientation)
        {
            var free = true;

            switch (shipOrientation)
            {
                case ShipOrientation.Vertical:
                    free = this.IsFree(index.X - 1, index.Y + 1);
                    break;

                case ShipOrientation.Horizontal:
                    free = this.IsFree(index.X - 1, index.Y + shipType.ToInt());
                    break;
            }

            return free;
        }

        /// <summary>
        /// Проверка свободно ли низ лево
        /// </summary>
        /// <param name="index">Индекс</param>
        /// <param name="shipType">Тип корабля</param>
        /// <param name="shipOrientation">Положение корабля</param>
        /// <returns>Свободно ли низ лево</returns>
        private bool FreeBottomLeft(Point index, ShipType shipType, ShipOrientation shipOrientation)
        {
            var free = true;

            switch (shipOrientation)
            {
                case ShipOrientation.Vertical:
                    free = this.IsFree(index.X + shipType.ToInt(), index.Y - 1);
                    break;

                case ShipOrientation.Horizontal:
                    free = this.IsFree(index.X + 1, index.Y - 1);
                    break;
            }

            return free;
        }

        /// <summary>
        /// Проверка свободно ли низ право
        /// </summary>
        /// <param name="index">Индекс</param>
        /// <param name="shipType">Тип корабля</param>
        /// <param name="shipOrientation">Положение корабля</param>
        /// <returns>Свободно ли низ право</returns>
        private bool FreeBottomRight(Point index, ShipType shipType, ShipOrientation shipOrientation)
        {
            var free = true;

            switch (shipOrientation)
            {
                case ShipOrientation.Vertical:
                    free = this.IsFree(index.X + shipType.ToInt(), index.Y + 1);
                    break;

                case ShipOrientation.Horizontal:
                    free = this.IsFree(index.X + 1, index.Y + shipType.ToInt());
                    break;
            }

            return free;
        }


        /// <summary>
        /// Есть ли свободный ход в указанной позиции
        /// </summary>
        /// <param name="i">i</param>
        /// <param name="j">j</param>
        /// <returns>Свободно?</returns>
        private bool IsFree(int i, int j)
        {
            return !this.gameField[i, j];
        }
    }
}