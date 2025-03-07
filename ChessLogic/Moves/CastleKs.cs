﻿namespace ChessLogic.Moves;

public class CastleKs : Move
{
    public CastleKs(Position fromPos, Position toPos)
    {
        FromPos = fromPos;
        ToPos = toPos;
    }

    public override MoveType Type => MoveType.CastleKs;

    public override Position FromPos { get; }

    public override Position ToPos { get; }


    public override void Execute(Board board)
    {
        var rook = board[ToPos + Direction.East];
        rook.HasMoved = true;
        board[FromPos + Direction.East] = rook;
        board[ToPos + Direction.East] = null;


        var king = board[FromPos];
        king.HasMoved = true;
        board[ToPos] = king;
        board[FromPos] = null;
    }
}