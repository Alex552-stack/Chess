using ChessLogic;
using ChessLogic.Moves;
using ChessServer.Models;

namespace ChessServer.Services;

public class MoveValidatorService : IMoveValidatorService
{
    private readonly IChesService _chessService;

    public MoveValidatorService(IChesService chessService)
    {
        _chessService = chessService;
    }

    public async Task<Result<Move, string>> ValidateMove(MoveDto move, Player playerColor, string gameId)
    {
        
        var fromPozition = MoveDto.ConvertToPosition(move.From);
        if (fromPozition == null)
        {
            return Result<Move,string>.Fail("From pozition is null");
        }
        
        var gameState = _chessService.GetGameState(gameId);
        if (gameState.CurrentPlayer != playerColor)
        {
            return Result<Move, string>.Fail("Current player color is wrong");
        }
        if (gameState.GetPieceAt(fromPozition) == null || gameState.GetPieceAt(fromPozition).Color != playerColor)
        {
            return Result<Move, string>.Fail("Current piece does not exist or color is wrong");
        }

        var toPosition = MoveDto.ConvertToPosition(move.To);
        foreach (var legalMove in gameState.LegalMovesForPiece(fromPozition))
        {
            if (legalMove.ToPos == toPosition)
            {
                return Result<Move, string>.Ok(legalMove);
            }
        }
        
        return Result<Move, string>.Fail("Move was not legal");
    }
}