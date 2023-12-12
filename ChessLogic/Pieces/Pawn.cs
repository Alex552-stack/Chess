using ChessLogic.Moves;

namespace ChessLogic
{
    public class Pawn : Piece
    {
        public override PieceType Type => PieceType.Pawn;

        public override Player Color { get; }

        private readonly Direction forward;

        public Pawn(Player color)
        {
            Color = color;
            if (color == Player.White)
                forward = Direction.North;
            else if (color == Player.Black)
                forward = Direction.Sounth;
        }

        public override Piece Copy()
        {
            Pawn copy = new Pawn(Color);
            copy.HasMoved = HasMoved;
            return copy;
        }

        private static bool CanMoveTo(Position pos, Board board)
        {
            return Board.IsInside(pos) && board.IsEmpty(pos);
        }


        private bool CanCaptureAt(Position pos, Board board)
        {
            if(!Board.IsInside(pos) || board.IsEmpty(pos)) 
                return false;

            return board[pos].Color != Color;
        }

        private IEnumerable<Move> ForwardMoves(Position from, Board board)
        {
            Position oneMovePos = from + forward;
            if(CanMoveTo(oneMovePos, board))
            {
                if (board.IsPosOnTheEdge(oneMovePos))
                {
                    yield return new PawnPromotion(from, oneMovePos);
                    yield break;
                }
                else
                {
                    yield return new NormalMove(from, oneMovePos);

                    Position twoMovePos = oneMovePos + forward;
                    if (!HasMoved && CanMoveTo(twoMovePos, board))
                    {
                        yield return new NormalMove(from, twoMovePos);
                    }
                }
            }
        }

        private IEnumerable<Move> DiagonalMove(Position from, Board board)
        {

            foreach(Direction dir in new Direction[] {Direction.West, Direction.East})
            {
                Position to = from + forward + dir;

                if(CanCaptureAt(to, board))
                {
                    if(board.IsPosOnTheEdge(to))
                        yield return new PawnPromotion(from, to);
                    else
                        yield return new NormalMove(from, to);
                }
            }
        }

        private IEnumerable<Move> EnPassant(Position from, Board board)
        {
            if(board.WasLastPieceMovedPawn && board.WasLastMoveDouble)
            {
                foreach (Direction dir in new Direction[] { Direction.West, Direction.East })
                {
                    if(!board.IsEmpty(from + dir) 
                        && board.LastPawnMoved == from + dir 
                        && board[from + dir].Color != Color)
                    {
                        yield return new EnPassant(from, from + forward + dir);
                    }
                }
            }
        }

        public override IEnumerable<Move> GetMoves(Position from, Board board)
        {
            return ForwardMoves(from,board).Concat(DiagonalMove(from, board)).Concat(EnPassant(from, board));
        }

        public override bool CanCaptureOpponentKing(Position from, Board board)
        {
            return DiagonalMove(from, board).Any(move => board[move.ToPos] != null && board[move.ToPos].Type == PieceType.King);
        }
    }
}
