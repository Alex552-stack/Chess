﻿using ChessLogic.Moves;

namespace ChessLogic.Pieces;

public class Rook : Piece
{
    public Rook(Player color)
    {
        Color = color;
    }

    public override PieceType Type => PieceType.Rook;

    public override Player Color { get; }

    private static readonly Direction[] Dirs = new Direction[]
    {
        Direction.North,
        Direction.East,
        Direction.West,
        Direction.Sounth
    };

    public override Piece Copy()
    {
        var copy = new Rook(Color);
        copy.HasMoved = HasMoved;
        return copy;
    }

    public override IEnumerable<Move> GetMoves(Position from, Board board)
    {
        return MovePositionInDir(from, board, Dirs).Select(to => new NormalMove(from, to));
    }
}