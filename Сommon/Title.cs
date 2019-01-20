namespace Сommon
{
    using System.Drawing;

    /// <summary>
    /// Класс заголовка
    /// </summary>
    public class Title
    {
        /// <summary>
        /// Текст
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// Шрифт
        /// </summary>
        public Font Font { get; set; }

        /// <summary>
        /// Кисть
        /// </summary>
        public Brush Brush { get; set; }

        /// <summary>
        /// Прямоугольник в котором рисовать
        /// </summary>
        public ResizableRectangle Rectangle { get; set; }

        public Title(string text, Font font, Brush brush, ResizableRectangle rectangle)
        {
            this.Text = text;
            this.Font = font;
            this.Brush = brush;
            this.Rectangle = rectangle;
        }
    }
}