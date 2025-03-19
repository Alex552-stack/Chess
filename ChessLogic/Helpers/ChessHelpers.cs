using System.Text;
using ChessLogic.Pieces;

namespace ChessLogic.Helpers;

public static class ChessHelpers
{
    public static string GetFenNotation(this GameState gameState)
    {
        var sb = new StringBuilder();
        var nullCtr = 0;

        for (var i = 0; i < Board.Size; i++)
        {
            for (var j = 0; j < Board.Size; j++)
            {
                if (gameState.Board[i, j] == null)
                    nullCtr++;
                else
                {
                    if (nullCtr != 0)
                    {
                        sb.Append(nullCtr);
                        nullCtr = 0;
                    }
                    sb.Append(gameState.Board[i, j].GetPieceTypeByColor());
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
        sb.Append(gameState.CurrentPlayer == Player.White ? 'w' : 'b'); // Assume gameState.Board.IsWhiteTurn tracks the active color

        // Castling availability (KQkq or -)
        sb.Append(' ');
        var castlingRights = "-";
        // if (gameState.Board.CanWhiteCastleKingSide) castlingRights += "K";
        // if (gameState.Board.CanWhiteCastleQueenSide) castlingRights += "Q";
        // if (gameState.Board.CanBlackCastleKingSide) castlingRights += "k";
        // if (gameState.Board.CanBlackCastleQueenSide) castlingRights += "q";
        // sb.Append(string.IsNullOrEmpty(castlingRights) ? "-" : castlingRights);
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

        public static char GetPieceTypeByColor(this Piece piece)
        {
            return piece.Color switch
            {
                Player.None => throw new Exception("The piece did not belong to any player"),
                Player.White => char.ToUpper((char)piece.Type),
                _ => (char)piece.Type
            };
        }

}