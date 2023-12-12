namespace ChessLogic.Moves
{
    public class PawnPromotion : Move
    {
        public PawnPromotion(Position fromPos, Position toPos)
        {
            FromPos = fromPos;
            ToPos = toPos;
            //PromoteTo 
        }

        public override MoveType Type => MoveType.PawnPromotion;

        public override Position FromPos { get; }

        public override Position ToPos { get; }

        private Piece CreatePiece(Player color, PieceType t)
        {
            return t switch
            {
                PieceType.Rook => new Rook(color),
                PieceType.Bishop => new Bishop(color),
                PieceType.Queen => new Queen(color),
                PieceType.Knight => new Knight(color),
                _ => throw new NotImplementedException()
            };
        }

        public override void Execute(Board board)
        {
            Piece piece = board[FromPos];
            board[ToPos] = piece;
            board[FromPos] = null;
            piece.HasMoved = true;
        }

        public void Promote(Board board, PieceType type)
        {
            Player color = board[ToPos].Color;
            Piece piece = CreatePiece(color, type);
            board[ToPos] = piece;
        }
    }
}
