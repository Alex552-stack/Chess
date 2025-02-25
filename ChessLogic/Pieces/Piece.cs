using ChessLogic.Moves;

namespace ChessLogic.Pieces;

public abstract class Piece
{
    public abstract PieceType Type { get; }
    public abstract Player Color { get; }
    public bool HasMoved { get; set; } = false;

    public override int GetHashCode()
    {
        return HashCode.Combine(Type, Color, HasMoved);
    }

    public abstract Piece Copy();

    public abstract IEnumerable<Move> GetMoves(Position from, Board board);

    protected IEnumerable<Position> MovePositionInDir(Position from, Board board, Direction dir)
    {
        for (var pos = from + dir; Board.IsInside(pos); pos += dir)
        {
            if (board.IsEmpty(pos))
            {
                yield return pos;
                continue;
            }

            var piece = board[pos];

            if (piece.Color != Color) yield return pos;

            yield break;
        }
    }

    protected IEnumerable<Position> MovePositionInDir(Position from, Board board, Direction[] dir)
    {
        return dir.SelectMany(dir => MovePositionInDir(from, board, dir));
    }

    public virtual bool CanCaptureOpponentKing(Position from, Board board)
    {
        return GetMoves(from, board).Any(move => board[move.ToPos] != null && board[move.ToPos].Type == PieceType.King);
    }
}