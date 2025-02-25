namespace ChessLogic;

public class Direction
{
    public static readonly Direction North = new(-1, 0);
    public static readonly Direction Sounth = new(1, 0);
    public static readonly Direction East = new(0, 1);
    public static readonly Direction West = new(0, -1);
    public static readonly Direction NorthEast = North + East;
    public static readonly Direction NorthWest = North + West;
    public static readonly Direction SouthEast = Sounth + East;
    public static readonly Direction SouthWest = Sounth + West;

    public int ColumnDelta { get; }
    public int RowDelta { get; }

    public Direction(int rowDelta, int columnDelta)
    {
        RowDelta = rowDelta;
        ColumnDelta = columnDelta;
    }

    public static Direction operator +(Direction dir1, Direction dir2)
    {
        return new Direction(dir1.RowDelta + dir2.RowDelta, dir1.ColumnDelta + dir2.ColumnDelta);
    }

    public static Direction operator *(int scalar, Direction dir)
    {
        return new Direction(scalar * dir.RowDelta, scalar * dir.ColumnDelta);
    }
}