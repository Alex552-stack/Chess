
using System.Runtime.Serialization;

namespace ChessLogic
{
	[DataContract]
	public class Board
    {
		[DataMember]
		public Piece[,] pieces { get; set; } = new Piece[8, 8];
		[DataMember]
		public bool WasLastPieceMovedPawn { get; set; }
		[DataMember]
		public Position LastPawnMoved;
		[DataMember]
		public bool WasLastMoveDouble { get; set; }

        public Piece this[int row, int col]
        {
            get { return pieces[row, col]; }
            set { pieces[row, col] = value; }
        }

        public Piece this[Position pos]
        {
            get { return pieces[pos.Row, pos.Column]; }
            set { this[pos.Row, pos.Column] = value; }
        }

        public override int GetHashCode()
        {
            int hashCode = 0;
            for(int i = 0; i < 8; i++)
            {
                for(int j = 0; j < 8; j++)
                {
                    hashCode = HashCode.Combine(hashCode.GetHashCode(), pieces[i, j]);
                }
            }
            return hashCode;
        }

        public static Board Initial()
        {
            Board board = new Board();
            board.AddStartPieces();
            return board;
        }

        private void AddStartPieces()
        {
            this[0, 0] = new Rook(Player.Black);
            this[0, 1] = new Knight(Player.Black);
            this[0,2] = new Bishop(Player.Black);
            this[0, 3] = new Queen(Player.Black);
            this[0, 4] = new King(Player.Black);
            this[0, 5] = new Bishop(Player.Black);
            this[0, 6] = new Knight(Player.Black);
            this[0, 7] = new Rook(Player.Black);

            this[7, 0] = new Rook(Player.White);
            this[7, 1] = new Knight(Player.White);
            this[7, 2] = new Bishop(Player.White);
            this[7, 3] = new Queen(Player.White);
            this[7, 4] = new King(Player.White);
            this[7, 5] = new Bishop(Player.White);
            this[7, 6] = new Knight (Player.White);
            this[7, 7] = new Rook (Player.White);

            for(int  i = 0; i < 8;i++)
            {
                this[1, i] = new Pawn(Player.Black);
                this[6,i] = new Pawn(Player.White);
            }
        }

        public static bool IsInside(Position pos)
        {
            return pos.Row >= 0 && pos.Row < 8 && pos.Column >= 0 && pos.Column < 8;
        }

        public bool IsEmpty(Position pos)
        {
            return this[pos] == null;
        }

        public int CountPieces()
        {
            int sum = 0;
            for (int i = 0; i < 8; i++)
                for (int j = 0; j < 8; j++)
                    if (this[i, j] != null)
                        sum++;
            return sum;
        }

        private IEnumerable<Position> GetAllPiecePositions()
        {
            for (int i = 0; i < 8; i++)
            {
                for(int j = 0; j < 8; j++)
                {
                    Position pos = new Position(i, j);
                    if (!IsEmpty(pos))
                        yield return pos;
                }
            }
        }

        public IEnumerable<Position> GetPiecePositionsOf(Player player)
        {
            return GetAllPiecePositions().Where(pos => this[pos].Color == player);
        }

        public bool LeavesOwnKingInCheck(Player player)
        {
            return GetPiecePositionsOf(player.Opponent()).Any(pos => this[pos].CanCaptureOpponentKing(pos, this));
        }

        public bool IsPosOnTheEdge(Position position)
        {
            return position.Row == 0 || position.Row == 7;
        }

        public int PieceValue(Position pos)
        {
            return this[pos].Type switch
            {
                PieceType.Pawn =>  10,
                PieceType.Queen => 10,
                PieceType.Rook => 5,
                PieceType.Bishop => 4,
                PieceType.Knight => 4,
                _ => 0
            };
        }
        
        private int MaterialOf(Player player)
        {
            return GetPiecePositionsOf(player).Select(pos => PieceValue(pos)).Sum();
        }

        public bool IsSufficientMaterial(Player player)
        {
            if (MaterialOf(player) < 6)
                return false;
            return true;
        }

        public Board Copy()
        {
            Board board = new Board();
            foreach(Position pos in GetAllPiecePositions())
            {
                board[pos] = this[pos].Copy();
            }
            return board;
        }

        
    }
}
