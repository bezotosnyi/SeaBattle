namespace Сommon
{
    using System.Drawing;

    /// <summary>
    /// Класс изменяюмого прямогульника
    /// </summary>
    public class ResizableRectangle
    {
        /// <summary>
        /// Верхний левый угол X
        /// </summary>
        public float X { get; set; }

        /// <summary>
        /// Верхний левый угол Y
        /// </summary>
        public float Y { get; set; }

        /// <summary>
        /// Ширина
        /// </summary>
        public float Width { get; set; }

        /// <summary>
        /// Высота
        /// </summary>
        public float Height { get; set; }

        public ResizableRectangle()
        {
        }

        public ResizableRectangle(float x, float y, float width, float height)
        {
            this.X = x;
            this.Y = y;
            this.Width = width;
            this.Height = height;
        }

        // оператор явного перевода из ResizableRectangle в RectangleF
        public static explicit operator RectangleF(ResizableRectangle rectangle)
        {
            return new RectangleF(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height);
        }
    }
}