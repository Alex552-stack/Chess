using System.Text;
using ChessLogic.Helpers;
using ChessLogic.Moves;
using ChessLogic.Pieces;

namespace ChessLogic;

public class GameState
{
    public Board Board { get; set; }
    public Player CurrentPlayer { get; private set; }

    public State State = State.Playing;

    private int PieceCount { get; set; } = 0;

    private int MovesWithotProgres { get; set; }

    private Dictionary<int, int> _hashCounts = new();

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

    public bool IsMoveLeavingOwnKingInCheck(Move move)
    {
        var copy = Board.Copy();
        move.Execute(copy);
        return copy.LeavesOwnKingInCheck(CurrentPlayer);
    }

    public IEnumerable<Move> LegalMovesForPiece(Position pos)
    {
        if (Board.IsEmpty(pos) || Board[pos].Color != CurrentPlayer)
            return [];

        var piece = Board[pos];
        return piece.GetMoves(pos, Board).Where(move => !IsMoveLeavingOwnKingInCheck(move));
    }

    public IEnumerable<Move> LegalMovesForCurrentPlayer()
    {
        return Board.GetPiecePositionsOf(CurrentPlayer).SelectMany(pos => LegalMovesForPiece(pos));
    }

    private bool ThreeRepetitions()
    {
        var hashCode = Board.GetHashCode();
        if (_hashCounts.TryGetValue(hashCode, out var count))
        {
            count++;
            _hashCounts[hashCode] = count;
            if (count == 3)
                return true;
        }
        else
        {
            _hashCounts.Add(hashCode, 1);
        }

        return false;
    }

    private void IncrementMovesWithoutProgress()
    {
        if (PieceCount != Board.CountPieces() && Board.WasLastPieceMovedPawn)
            MovesWithotProgres = 0;
        else
            MovesWithotProgres++;
        PieceCount = Board.CountPieces();
    }

    private void CheckForGameOver()
    {
        IncrementMovesWithoutProgress();
        /*if (Moves.Count == 5 && Moves[0] == Moves[2] && Moves[2] == Moves[4] && Moves[1] == Moves[3])
        {
            Results = Results.Draw(EndReason.ThreeRepetitions);
        }*/

        if (MovesWithotProgres == 50)
        {
            Results = Results.Draw(EndReason.FiftyMoveRule);
            return;
        }

        if (!Board.IsSufficientMaterial(CurrentPlayer) && !Board.IsSufficientMaterial(CurrentPlayer.Opponent()))
        {
            Results = Results.Draw(EndReason.InsufficientMaterial);
            return;
        }

        if (ThreeRepetitions())
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
            {
                Board.WasLastMoveDouble = false;
            }
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
    
    public string GetFenNotation()
    {
        var sb = new StringBuilder();
        var nullCtr = 0;

        for (var i = 0; i < Board.Size; i++)
        {
            for (var j = 0; j < Board.Size; j++)
            {
                if (Board[i, j] == null)
                    nullCtr++;
                else
                {
                    if (nullCtr != 0)
                    {
                        sb.Append(nullCtr);
                        nullCtr = 0;
                    }
                    sb.Append(Board[i, j].GetPieceTypeByColor());
                }
            }

            if (nullCtr != 0)
            {
                sb.Append(nullCtr);
                nullCtr = 0;
            }

            if (i < Board.Size - 1) // Avoid trailing "/"
                sb.Append('/');
        }
        sb.Append(' ');
        sb.Append(CurrentPlayer == Player.White ? 'w' : 'b'); // Assume gameState.Board.IsWhiteTurn tracks the active color

        // Castling availability (KQkq or -)
        // sb.Append(' ');
        var castlingRights = " - - 0 1";
        // if (gameState.Board.CanWhiteCastleKingSide) castlingRights += "K";
        // if (gameState.Board.CanWhiteCastleQueenSide) castlingRights += "Q";
        // if (gameState.Board.CanBlackCastleKingSide) castlingRights += "k";
        // if (gameState.Board.CanBlackCastleQueenSide) castlingRights += "q";
        sb.Append(string.IsNullOrEmpty(castlingRights) ? "-" : castlingRights);
        //
        // // En passant target square (e3 or -)
        // sb.Append(' ');
        // sb.Append(gameState.Board.EnPassantTargetSquare ?? "-"); // Assume this is stored or computed as gameState.Board.EnPassantTargetSquare
        //
        // // Halfmove clock
        // sb.Append(' ');
        // sb.Append(gameState.Board.HalfmoveClock); // Assume gameState.Board.HalfmoveClock tracks halfmoves
        //
        // // Fullmove number
        // sb.Append(' ');
        // sb.Append(gameState.Board.FullmoveNumber); // Assume gameState.Board.FullmoveNumber tracks fullmove number
        //
        // return sb.ToString();

        return sb.ToString();
    }

    public Piece GetPieceAt(Position position)
    {
        return Board[position];
    }
}