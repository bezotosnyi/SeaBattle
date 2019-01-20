namespace Сommon
{
    using System.Drawing;

    /// <summary>
    /// Разметка поля
    /// </summary>
    public class GameLayout
    {
        /// <summary>
        /// Массив координат линиий сетки
        /// </summary>
        private readonly Line[] gridLines = new Line[2 * Constants.Size];

        /// <summary>
        /// Надписи
        /// </summary>
        private readonly Title[] titles = new Title[2 * Constants.Size];

        /// <summary>
        /// Границы игрового поля
        /// </summary>
        private readonly ResizableRectangle border = new ResizableRectangle();

        /// <summary>
        /// Кисть для рисования сетки
        /// </summary>
        private readonly Pen gridPen = new Pen(Color.Black, 1f);

        /// <summary>
        /// Кисть для рисования границ поля
        /// </summary>
        private readonly Pen borderPen = new Pen(Color.Black);

        /// <summary>
        /// Цвет текста
        /// </summary>
        private readonly Brush titleBrush = Brushes.Black;

        /// <summary>
        /// Шрифт
        /// </summary>
        private readonly FontFamily fontFamily = new FontFamily("Times New Roman");

        /// <summary>
        /// Формат текста
        /// </summary>
        private readonly StringFormat titleFormat = new StringFormat
                                                        {
                                                            Alignment = StringAlignment.Center,
                                                            LineAlignment = StringAlignment.Center
                                                        };

        /// <summary>
        /// Расчет игрового поля
        /// </summary>
        /// <param name="width">Ширина окна</param>
        /// <param name="height">Высота окна</param>
        public void CalculationGameLayout(int width, int height)
        {
            // рассчитываем линии поля
            this.CalculationGridLines(width, height);

            // рассчитываем границы поля
            this.CalculationBorder(width, height);

            // рассчитываем заголовки
            this.CalculationTitles(width, height);
        }

        /// <summary>
        /// Рисование игрового поля
        /// </summary>
        /// <param name="graphics">Графика для рисования</param>
        public void Draw(Graphics graphics)
        {
            // рисуем линии
            foreach (var line in this.gridLines)
            {
                graphics.DrawLine(this.gridPen, line.StartPoint, line.EndPoint);
            }

            // рисуем границу
            graphics.DrawRectangles(this.borderPen, new[] { (RectangleF)this.border });

            // рисуем заголовки
            foreach (var title in this.titles)
            {
                graphics.DrawString(
                    s: title.Text,
                    font: title.Font,
                    brush: this.titleBrush,
                    layoutRectangle: (RectangleF)title.Rectangle,
                    format: this.titleFormat);
            }
        }

        /// <summary>
        /// Расчет линий игрового поля
        /// </summary>
        /// <param name="width">Ширина</param>
        /// <param name="height">Высота</param>
        private void CalculationGridLines(int width, int height)
        {
            for (int i = 0; i < this.gridLines.Length; i++)
            {
                if (i < this.gridLines.Length / 2)
                {
                    // горизонтальные линии
                    this.gridLines[i] = new Line(
                        new PointF(0f, (i + 1) * height / (this.gridLines.Length / 2f + 1)),
                        new PointF(width, (i + 1) * height / (this.gridLines.Length / 2f + 1)));
                }
                else
                {
                    // вертикальные линии
                    this.gridLines[i] = new Line(
                        new PointF((i - (this.gridLines.Length / 2f - 1)) * width / (this.gridLines.Length / 2f + 1), 0f),
                        new PointF((i - (this.gridLines.Length / 2f - 1)) * width / (this.gridLines.Length / 2f + 1), height));
                }
            }
        }

        /// <summary>
        /// Расчет границы игрового поля
        /// </summary>
        /// <param name="width">Ширина</param>
        /// <param name="height">Высота</param>
        private void CalculationBorder(int width, int height)
        {
            this.border.X = width / (this.gridLines.Length / 2f + 1);
            this.border.Y = height / (this.gridLines.Length / 2f + 1);
            this.border.Width = width - width / (0.974f * (this.gridLines.Length / 2f + 1));
            this.border.Height = height - height / (0.974f * (this.gridLines.Length / 2f + 1));
            this.borderPen.Width = (width + height) / (Constants.Size * 20f);
        }
        
        /// <summary>
        /// Расчет заголовков
        /// </summary>
        /// <param name="width">Ширина</param>
        /// <param name="height">Высота</param>
        private void CalculationTitles(int width, int height)
        {
            for (int i = 0; i < this.titles.Length; i++)
            {
                // горизонтальные заголовки
                if (i < this.titles.Length / 2)
                {
                    // русская буква с unicode
                    var text = $"{(char)(i + 1040)}";
                    this.titles[i] = new Title(
                        text: text,
                        font: new Font(
                            family: this.fontFamily,
                            emSize: this.GetBestTextSize(
                                text: text,
                                width: width / (this.titles.Length / 2 + 1),
                                height: height / (this.titles.Length / 2 + 1)),
                            style: FontStyle.Bold),
                        brush: this.titleBrush,
                        rectangle: new ResizableRectangle(
                            x: (i + 1) * width / (this.titles.Length / 2f + 1),
                            y: 0,
                            width: width / (this.titles.Length / 2f + 1),
                            height: height / (this.titles.Length / 2f + 1)));
                }
                else
                {
                    // вертикальные заголовки (цифры)
                    var text = $"{i - this.titles.Length / 2 + 1}";
                    this.titles[i] = new Title(
                        text: text,
                        font: new Font(
                            family: this.fontFamily,
                            emSize: this.GetBestTextSize(
                                text: text,
                                width: width / (this.titles.Length / 2 + 1),
                                height: height / (this.titles.Length / 2 + 1)),
                            style: FontStyle.Bold),
                        brush: this.titleBrush,
                        rectangle: new ResizableRectangle(
                            x: 0,
                            y: (i - (this.titles.Length / 2f - 1)) * height / (this.titles.Length / 2f + 1),
                            width: width / (this.titles.Length / 2f + 1),
                            height: height / (this.titles.Length / 2f + 1)));
                }
            }
        }

        /// <summary>
        /// Расчет 'идеального' размера текста под область
        /// </summary>
        /// <param name="text">Текст</param>
        /// <param name="width">Ширина</param>
        /// <param name="height">Высота</param>
        /// <returns>Идеальный размер текста</returns>
        private float GetBestTextSize(string text, int width, int height)
        {
            if (text.Length > 0)
            {
                using (var bitmap = new Bitmap(width, height))
                using (var graphics = Graphics.FromImage(bitmap))
                {
                    for (var i = 0.1f; i <= 100; i += 0.1f)
                    {
                        using (var font = new Font(this.fontFamily, i))
                        {
                            var textSize = graphics.MeasureString(text, font);

                            if (textSize.Width > width || textSize.Height > height)
                            {
                                return i == 0.1 ? 0.1f : i - 0.1f;
                            }
                        }
                    }
                }
            }

            return 0.1f;
        }
    }
}