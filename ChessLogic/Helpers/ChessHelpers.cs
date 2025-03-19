using ChessLogic.Pieces;

namespace ChessLogic.Helpers;

public static class ChessHelpers
{
    public static string GetFenNotation(this Board board)
    {
        string result = "";
        int nullCtr = 0;
        for (int i = 0; i < Board.Size; i++)
        {
            for (int j = 0; j < Board.Size; j++)
            {
                if (board[i, j] == null) nullCtr++;
                else
                {
                    if (nullCtr != 0)
                    {
                        result += $"{nullCtr}";
                        nullCtr = 0;
                    }

                    result += $"{board[i, j].GetPieceTypeByColor()}";
                }
            }
            
            if (nullCtr != 0)
            {
                result += $"{nullCtr}";
                nullCtr = 0;
            }
            result += "/";
        }

        return result;
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