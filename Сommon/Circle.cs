namespace Сommon
{
    /// <summary>
    /// Круг
    /// </summary>
    public class Circle
    {
        /// <summary>
        /// Прямоугольник, в который будет вписан круг
        /// </summary>
        public ResizableRectangle CircleRect { get; set; }

        public Circle(ResizableRectangle circleRect)
        {
            this.CircleRect = circleRect;
        }
    }
}