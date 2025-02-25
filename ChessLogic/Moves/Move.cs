namespace ChessLogic.Moves;

public abstract class Move
{
    public abstract MoveType Type { get; }
    public abstract Position FromPos { get; }
    public abstract Position ToPos { get; }

    public override int GetHashCode()
    {
        return HashCode.Combine(Type, FromPos, ToPos);
    }

    public abstract void Execute(Board board);

    public int AbsoluteColumnDistance()
    {
        return Math.Abs(FromPos.Row - ToPos.Row);
    }
}