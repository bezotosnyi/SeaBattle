namespace Сommon
{
    /*
        ----------------------------------------------------------
        |TopLeft   |          TopOther               |TopRight   |
        -----------+---------------------------------+------------
        |          |                                 |           |
        |          |                                 |           |
        |Left      |          Other                  |Right      |  
        |          |                                 |           |
        |          |                                 |           |
        -----------+---------------------------------+------------
        |BottomLeft|          BottomOther            |BottomRight|
        ----------------------------------------------------------
    */

    /// <summary>
    /// Позиция вставки корабля
    /// </summary>
    public enum PastingShipPosition
    {
        TopLeft,

        TopRight,

        TopOther,


        BottomLeft,

        BottomRight,

        BottomOther,


        Left,

        Right,

        Other
    }
}
