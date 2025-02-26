using System;
using System.Collections.Generic;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using ChessLogic;
using ChessLogic.Pieces;

namespace ChessUI;

public class ImageParser
{
    private readonly Dictionary<PieceType, ImageSource> _blackSources;
    private readonly Dictionary<PieceType, ImageSource> _whiteSources;

    public ImageParser()
    {
        _whiteSources = new Dictionary<PieceType, ImageSource>
        {
            { PieceType.Pawn, LoadImage("Assets/PawnW.png") },
            { PieceType.Bishop, LoadImage("Assets/BishopW.png") },
            { PieceType.Knight, LoadImage("Assets/KnightW.png") },
            { PieceType.Rook, LoadImage("Assets/RookW.png") },
            { PieceType.Queen, LoadImage("Assets/QueenW.png") },
            { PieceType.King, LoadImage("Assets/KingW.png") }
        };
        _blackSources = new Dictionary<PieceType, ImageSource>
        {
            { PieceType.Pawn, LoadImage("Assets/PawnB.png") },
            { PieceType.Bishop, LoadImage("Assets/BishopB.png") },
            { PieceType.Knight, LoadImage("Assets/KnightB.png") },
            { PieceType.Rook, LoadImage("Assets/RookB.png") },
            { PieceType.Queen, LoadImage("Assets/QueenB.png") },
            { PieceType.King, LoadImage("Assets/KingB.png") }
        };
    }

    private ImageSource LoadImage(string filePath)
    {
        return new BitmapImage(new Uri(filePath, UriKind.Relative));
    }

    public ImageSource GetImage(Player color, PieceType type)
    {
        return color switch
        {
            Player.White => _whiteSources[type],
            Player.Black => _blackSources[type],
            _ => null
        };
    }

    public ImageSource GetImage(Piece piece)
    {
        if (piece == null)
            return null;

        return GetImage(piece.Color, piece.Type);
    }
}