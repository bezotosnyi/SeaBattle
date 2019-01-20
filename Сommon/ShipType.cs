namespace Сommon
{
    using System;

    /// <summary>
    /// Тип корабля
    /// </summary>
    [Flags] // (используются биты для упрощения проверки возможности вставки)
    public enum ShipType
    {
        None = 0,

        One = 1,

        Two = 2 | One,

        Three = 4 | One | Two,

        Four = 8 | One | Two | Three
    }
}