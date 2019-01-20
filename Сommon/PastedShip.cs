namespace Сommon
{
    using System;

    /// <summary>
    /// Вставляемые корабли (контроль количества)
    /// </summary>
    public class PastedShip
    {
        private int one = 0;

        private int two = 0;

        private int three = 0;

        private int four = 0;

        public int One
        {
            get => this.one;

            set
            {
                if (this.one < Constants.MaxOne)
                    this.one = value;
                else
                    throw new Exception($"Максимум однопалубных кораблей {Constants.MaxOne}!");
            }
        }

        public int Two
        {
            get => this.two;

            set
            {
                if (this.two < Constants.MaxTwo)
                    this.two = value;
                else
                    throw new Exception($"Максимум двупалубных кораблей {Constants.MaxTwo}!");
            }
        }

        public int Three
        {
            get => this.three;

            set
            {
                if (this.three < Constants.MaxThree)
                    this.three = value;
                else
                    throw new Exception($"Максимум трёхпалубных кораблей {Constants.MaxThree}!");
            }
        }

        public int Four
        {
            get => this.four;

            set
            {
                if (this.four < Constants.MaxFour)
                    this.four = value;
                else
                    throw new Exception($"Максимум четырёхпалубных кораблей {Constants.MaxFour}!");
            }
        }
    }
}