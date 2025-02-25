using ChessLogic.Moves;

namespace ChessLogic.Pieces;

public class King : Piece
{
    public King(Player color)
    {
        Color = color;
    }

    public override PieceType Type => PieceType.King;

    public override Player Color { get; }

    private static readonly Direction[] Dirs = new Direction[]
    {
        Direction.North,
        Direction.East,
        Direction.West,
        Direction.Sounth,
        Direction.NorthEast,
        Direction.NorthWest,
        Direction.SouthEast,
        Direction.SouthWest
    };

    public override Piece Copy()
    {
        var copy = new King(Color);
        copy.HasMoved = HasMoved;
        return copy;
    }

    private IEnumerable<Position> MovePositions(Position from, Board board)
    {
        foreach (var dir in Dirs)
        {
            var to = from + dir;
            if (!Board.IsInside(to))
                continue;

            if (board.IsEmpty(to) || board[to].Color != Color) yield return to;
        }
    }


    private bool CanCastleKs(Position from, Board board)
    {
        return !HasMoved
               && board.IsEmpty(from + Direction.East)
               && board.IsEmpty(from + 2 * Direction.East)
               && !board.IsEmpty(from + 3 * Direction.East)
               && board[from + 3 * Direction.East].Type == PieceType.Rook
               && !board[from + 3 * Direction.East].HasMoved;
    }

    private bool CanCastleQs(Position from, Board board)
    {
        return !HasMoved
               && board.IsEmpty(from + Direction.West)
               && board.IsEmpty(from + 2 * Direction.West)
               && board.IsEmpty(from + 3 * Direction.West)
               && !board.IsEmpty(from + 4 * Direction.West)
               && board[from + 4 * Direction.West].Type == PieceType.Rook
               && !board[from + 4 * Direction.West].HasMoved;
    }

    private IEnumerable<Move> CastleMoves(Position from, Board board)
    {
        if (CanCastleKs(from, board))
            yield return new CastleKs(from, from + 2 * Direction.East);

        if (CanCastleQs(from, board))
            yield return new CastleQs(from, from + 2 * Direction.West);
    }


    public override IEnumerable<Move> GetMoves(Position from, Board board)
    {
        foreach (var to in MovePositions(from, board))
            yield return new NormalMove(from, to);

        foreach (var move in CastleMoves(from, board))
            yield return move;
    }
}