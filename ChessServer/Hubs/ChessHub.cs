using ChessLogic;
using ChessLogic.Helpers;
using ChessLogic.Moves;
using ChessServer.Models;
using ChessServer.Services;
using Microsoft.AspNetCore.SignalR;

namespace ChessServer.Hubs;

public class ChessHub : Hub
{
    private readonly ILogger<ChessHub> _logger;
    private readonly IMoveValidatorService _moveValidatorService;
    private readonly IChesService _chessService;

    public ChessHub(IChesService chessService, ILogger<ChessHub> logger, IMoveValidatorService moveValidatorService)
    {
        _logger = logger;
        _moveValidatorService = moveValidatorService;
        _chessService = chessService;
    }

    public async Task CreateGame()
    {
        var connectionId = Context.ConnectionId;
        var gameData = _chessService.StartGame();

        await Groups.AddToGroupAsync(connectionId, gameData[0]);
        await Clients.Group(gameData[0]).SendAsync("GameStarted", gameData[0], _chessService.GetState(gameData[0])?.GetFenNotation());
        await Clients.Caller.SendAsync("GetColor", gameData[1]);
    }

    public async Task<bool> MakeMoveWeb(string gameId, MoveDto move)
    {
        try
        {
            if (!_chessService.GameExists(gameId))
            {
                await Clients.Caller.SendAsync("Error", "Game not found.");
                return false;
            }
            //temporary solution because it seems that i do not save anywhere what color the players are
            var playerColor = _chessService.GetBoard(gameId)[MoveDto.ConvertToPosition(move.From)].Color;
            
            var validateMoveResponse = await _moveValidatorService.ValidateMove(move, playerColor, gameId);
            if (!validateMoveResponse.IsSuccess)
            {
                _logger.LogWarning($"GameId: {gameId}: {validateMoveResponse.Error}");
                return false;
            }
            
            var success = _chessService.MakeMove(gameId, validateMoveResponse.Success!);
            if (!success)
            {
                await Clients.Group(gameId).SendAsync("GameOver", _chessService.GetResults(gameId));
                _chessService.RemoveGame(gameId);
                await Clients.Group(gameId).SendAsync("Disconnect");
            }
            else
            {
                await UpdateBoard(gameId);
            }
        }
        catch (Exception ex)
        {
            await Clients.Caller.SendAsync("Error", $"Unexpected error: {ex.Message}");
            return false;
        }

        return true;
    }
    
    //For WPF app
    public async Task MakeMove(string gameId, Move move)
    {
        try
        {
            if (!_chessService.GameExists(gameId))
            {
                await Clients.Caller.SendAsync("Error", "Game not found.");
                return;
            }

            var success = _chessService.MakeMove(gameId, move);
            if (!success)
            {
                await Clients.Group(gameId).SendAsync("GameOver", _chessService.GetResults(gameId));
                _chessService.RemoveGame(gameId);
                await Clients.Group(gameId).SendAsync("Disconnect");
            }
            else
            {
                await UpdateBoard(gameId);
            }
        }
        catch (Exception ex)
        {
            await Clients.Caller.SendAsync("Error", $"Unexpected error: {ex.Message}");
        }
    }

    public async Task Disconnect(string gameId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, gameId);
    }

    public async Task<Move[]> LegalMovesForPiece(string gameId, Position pos)
    {
        if (!_chessService.GameExists(gameId))
        {
            await Clients.Caller.SendAsync("Error", "Game not found.");
            return Array.Empty<Move>();
        }

        return _chessService.GetPossibleMoves(gameId, pos);
    }

    public async Task PromotePawn(string gameId, PawnPromotion pawnPromotion, PieceType promotion)
    {
        if (!_chessService.GameExists(gameId))
        {
            await Clients.Caller.SendAsync("Error", "Game not found.");
            return;
        }

        _chessService.PromotePawn(gameId, pawnPromotion, promotion);
        await UpdateBoard(gameId);
    }

    private async Task UpdateBoard(string gameId)
    {
        if (!_chessService.GameExists(gameId)) return;
        await Clients.Group(gameId).SendAsync("UpdateBoard", _chessService.GetState(gameId)?.GetFenNotation());
    }
}