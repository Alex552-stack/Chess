namespace ChessLogic
{
    public class EnPassant : Move
    {
        public EnPassant(Position fromPos, Position toPos)
        {
            FromPos = fromPos;
            ToPos = toPos;
        }

        public override MoveType Type => MoveType.EnPassant;

        public override Position FromPos { get; }

        public override Position ToPos { get; }

        public override void Execute(Board board)
        {
            Piece pawn = board[FromPos];
            board[ToPos] = pawn;
            board[FromPos] = null;
            board[FromPos.Row, ToPos.Column] = null;
        }
    }
}
