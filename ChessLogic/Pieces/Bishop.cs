using ChessLogic.Moves;

namespace ChessLogic.Pieces;

public class Bishop : Piece
{
    public Bishop(Player color)
    {
        Color = color;
    }

    public override PieceType Type => PieceType.Bishop;

    public override Player Color { get; }

    private static readonly Direction[] Dirs = new Direction[]
    {
        Direction.NorthEast,
        Direction.SouthWest,
        Direction.NorthWest,
        Direction.SouthEast
    };


    public override Piece Copy()
    {
        var copy = new Bishop(Color);
        copy.HasMoved = HasMoved;
        return copy;
    }

    public override IEnumerable<Move> GetMoves(Position from, Board board)
    {
        return MovePositionInDir(from, board, Dirs).Select(to => new NormalMove(from, to));
    }
}