using ChessLogic.Moves;

namespace ChessLogic
{
    public class GameState
    {
        public Board Board { get; set; }
        public Player CurrentPlayer { get; private set; }

        public State state = State.Playing;

        private int PieceCount { get; set; } = 0;

        private int MovesWithotProgres { get; set; }

        Dictionary<int, int> HashCounts = new Dictionary<int, int>();

        public Results Results { get; set; } = null;

		public int PlayersCount { get; set; }

        public TimeSpan WhiteTime { get; set; }

        public TimeSpan BlackTime { get; set; }

		public GameState(Player player, Board board)
        {
            CurrentPlayer = player;
            Board = board;
            PieceCount = Board.CountPieces();
			PlayersCount = 1;
		}

        private bool IsMoveLeavingOwnKingInCheck(Move move)
        {
            Board copy = Board.Copy();
            move.Execute(copy);
            return copy.LeavesOwnKingInCheck(CurrentPlayer);
        }

        public IEnumerable<Move> LegalMovesForPiece(Position pos)
        {
            if (Board.IsEmpty(pos) || Board[pos].Color != CurrentPlayer)
                return Enumerable.Empty<Move>();

            Piece piece = Board[pos];
            return piece.GetMoves(pos, Board).Where(move => !IsMoveLeavingOwnKingInCheck(move));
        }

        public IEnumerable<Move> LegalMovesForCurrentPlayer()
        {
            return Board.GetPiecePositionsOf(CurrentPlayer).SelectMany(pos => LegalMovesForPiece(pos));
        }

        public bool ThreeRepetitions() 
        {
            int hashCode = Board.GetHashCode();
            if(HashCounts.TryGetValue(hashCode, out int count) )
            {
                count++;
                HashCounts[hashCode] = count;
                if (count == 3)
                    return true;
            }
            else
            {
                HashCounts.Add(hashCode, 1);
            }
            return false;
        }

        private void IncrementMovesWithoutProgress()
        {
            if (PieceCount != Board.CountPieces() && Board.WasLastPieceMovedPawn)
            {
                MovesWithotProgres = 0;
            }
            else
            {
                MovesWithotProgres++;
            }
            PieceCount = Board.CountPieces();
        }

        private void CheckForGameOver()
        {
            IncrementMovesWithoutProgress();
            /*if (Moves.Count == 5 && Moves[0] == Moves[2] && Moves[2] == Moves[4] && Moves[1] == Moves[3])
            {
                Results = Results.Draw(EndReason.ThreeRepetitions);
            }*/

            if(MovesWithotProgres == 50)
            {
                Results = Results.Draw(EndReason.FiftyMoveRule);
                return;
            }

            if (!Board.IsSufficientMaterial(CurrentPlayer) && !Board.IsSufficientMaterial(CurrentPlayer.Opponent()))
                {
                Results = Results.Draw(EndReason.InsufficientMaterial);
                return;
            }

            if(ThreeRepetitions())
            {
                Results = Results.Draw(EndReason.ThreeRepetitions);
                return;
            }

            if (!LegalMovesForCurrentPlayer().Any())
            {
                if (Board.LeavesOwnKingInCheck(CurrentPlayer))
                {
                    Results = Results.Win(CurrentPlayer.Opponent());
                    return;
                }
                else
                {
                    Results = Results.Draw(EndReason.Stalemate);
                    return;
                }
            }
        }

        public bool IsGameOver()
        {
            return Results != null;
        }

        /*public void ManageMoveList(Move move)
        {
            Moves.Add(move);
            if (Moves.Count == 6)
            {
                Moves.RemoveAt(5);
            }
        }*/

        private void PawnProgress(Move move)
        {
            if (Board[move.FromPos].Type == PieceType.Pawn)
            {
                Board.WasLastPieceMovedPawn = true;
                if (move.AbsoluteColumnDistance() == 2)
                { 
                    Board.WasLastMoveDouble = true;
                    Board.LastPawnMoved = move.ToPos;
                }
                else
                    Board.WasLastMoveDouble = false;
            }
            else
            {
                Board.WasLastPieceMovedPawn = false;
            }
        }

        public void MakeMove(Move move)
        {
            PawnProgress(move);
            move.Execute(Board);
            //ManageMoveList(move);
            CurrentPlayer = CurrentPlayer.Opponent();
            CheckForGameOver();
        }

        public void PromotePawn(PawnPromotion move, PieceType promotion)
        {
            move.Promote(Board, promotion);
        }
    }
}
