namespace ChessLogic.Moves;

public class CastleQs : Move
{
    public override MoveType Type => MoveType.CastleQs;

    public override Position FromPos { get; }

    public override Position ToPos { get; }

    public CastleQs(Position fromPos, Position toPos)
    {
        FromPos = fromPos;
        ToPos = toPos;
    }

    public override void Execute(Board board)
    {
        var rook = board[ToPos + 2 * Direction.West];
        rook.HasMoved = true;
        board[FromPos + Direction.West] = rook;
        board[ToPos + 2 * Direction.West] = null;


        var king = board[FromPos];
        king.HasMoved = true;
        board[ToPos] = king;
        board[FromPos] = null;
    }
}