using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessLogic.Moves
{
    public class CastleQs : Move
    {
        public override MoveType Type => MoveType.CastleQS;

        public override Position FromPos { get; }

        public override Position ToPos { get; }

        public CastleQs(Position fromPos, Position toPos)
        {
            FromPos = fromPos;
            ToPos = toPos;
        }

        public override void Execute(Board board)
        {
            Piece rook = board[ToPos + 2 * Direction.West];
            rook.HasMoved = true;
            board[FromPos + Direction.West] = rook;
            board[ToPos + 2 * Direction.West] = null;


            Piece king = board[FromPos];
            king.HasMoved = true;
            board[ToPos] = king;
            board[FromPos] = null;
        }
    }
}
