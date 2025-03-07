﻿using ChessLogic.Moves;

namespace ChessLogic.Pieces;

public class Knight : Piece
{
    public Knight(Player color)
    {
        Color = color;
    }

    public override PieceType Type => PieceType.Knight;

    public override Player Color { get; }

    public override Piece Copy()
    {
        var copy = new Knight(Color);
        copy.HasMoved = HasMoved;
        return copy;
    }

    private static IEnumerable<Position> PotentialToPositions(Position from)
    {
        foreach (var vDir in new Direction[] { Direction.North, Direction.Sounth })
        foreach (var hDir in new Direction[] { Direction.West, Direction.East })
        {
            yield return from + 2 * vDir + hDir;
            yield return from + 2 * hDir + vDir;
        }
    }

    private IEnumerable<Position> MovePosition(Position from, Board board)
    {
        return PotentialToPositions(from)
            .Where(pos => Board.IsInside(pos) && (board.IsEmpty(pos) || board[pos].Color != Color));
    }

    public override IEnumerable<Move> GetMoves(Position from, Board board)
    {
        return MovePosition(from, board).Select(to => new NormalMove(from, to));
    }
}