using ChessLogic.Moves;

namespace ChessLogic.Pieces;

public class Queen : Piece
{
    public Queen(Player color)
    {
        Color = color;
    }

    public override PieceType Type => PieceType.Queen;

    public override Player Color { get; }

    private static readonly Direction[] Dirs = new Direction[]
    {
        Direction.North,
        Direction.East,
        Direction.Sounth,
        Direction.West,
        Direction.NorthWest,
        Direction.NorthEast,
        Direction.SouthEast,
        Direction.SouthWest
    };

    public override Piece Copy()
    {
        var copy = new Queen(Color);
        copy.HasMoved = HasMoved;
        return copy;
    }

    public override IEnumerable<Move> GetMoves(Position from, Board board)
    {
        return MovePositionInDir(from, board, Dirs).Select(to => new NormalMove(from, to));
    }
}