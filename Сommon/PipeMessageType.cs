namespace Сommon
{
    using System;

    /// <summary>
    /// Тип сообщения
    /// </summary>
    [Serializable]
    public enum PipeMessageType
    {
        /// <summary>
        /// Расстановка закончена
        /// </summary>
        EndArrange,

        /// <summary>
        /// Установка хода
        /// </summary>
        SetMove,

        /// <summary>
        /// Ошибка
        /// </summary>
        Error,

        /// <summary>
        /// Обновление
        /// </summary>
        Update,

        /// <summary>
        /// Есть победитель
        /// </summary>
        Winn
    }
}
