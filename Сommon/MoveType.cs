namespace Сommon
{
    using System;

    /// <summary>
    /// Тип хода
    /// </summary>
    [Serializable]
    public enum MoveType
    {
        /// <summary>
        /// Попадание (крест)
        /// </summary>
        Cross,

        /// <summary>
        /// Промах (маленький круг)
        /// </summary>
        Circle
    }
}