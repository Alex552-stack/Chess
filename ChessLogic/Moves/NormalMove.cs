namespace ChessLogic.Moves;

public class NormalMove : Move
{
    public NormalMove(Position from, Position to)
    {
        FromPos = from;
        ToPos = to;
    }

    public override MoveType Type => MoveType.Normal;
    public override Position FromPos { get; }
    public override Position ToPos { get; }

    public override void Execute(Board board)
    {
        var piece = board[FromPos];
        board[ToPos] = piece;
        board[FromPos] = null;
        piece.HasMoved = true;
    }
}