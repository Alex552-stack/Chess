using ChessLogic;
using ChessLogic.Moves;
using ChessServer.Models;

namespace ChessServer.Services;

public interface IMoveValidatorService
{
    Task<Result<Move, string>> ValidateMove(MoveDto move, Player playerColor, string gameId);
}