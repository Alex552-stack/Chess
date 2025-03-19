using System.Text;
using ChessLogic.Pieces;

namespace ChessLogic.Helpers;

public static class ChessHelpers
{
    public static string GetFenNotation(this Board board)
    {
        var sb = new StringBuilder();
        var nullCtr = 0;

        for (var i = 0; i < Board.Size; i++)
        {
            for (var j = 0; j < Board.Size; j++)
            {
                if (board[i, j] == null)
                    nullCtr++;
                else
                {
                    if (nullCtr != 0)
                    {
                        sb.Append(nullCtr);
                        nullCtr = 0;
                    }
                    sb.Append(board[i, j].GetPieceTypeByColor());
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