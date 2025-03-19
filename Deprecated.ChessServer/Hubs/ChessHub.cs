using ChessLogic;
using ChessLogic.Moves;
using Deprecated.ChessServer.Services;
using Microsoft.AspNetCore.SignalR;

namespace Deprecated.ChessServer.Hubs
{
    public class ChessHub(ChessService chessService) : Hub
    {
        public async Task CreateGame()
        {
            var connectionId = Context.ConnectionId;
            var gameData = chessService.StartGame();
            
            await Groups.AddToGroupAsync(connectionId, gameData[0]);
            await Clients.Group(gameData[0]).SendAsync("GameStarted", gameData[0], chessService.GetBoard(gameData[0]));
            await Clients.Caller.SendAsync("GetColor", gameData[1]);
        }

        public async Task MakeMove(string gameId, Move move)
        {
            try
            {
                if (!chessService.GameExists(gameId))
                {
                    await Clients.Caller.SendAsync("Error", "Game not found.");
                    return;
                }

                var success = chessService.MakeMove(gameId, move);
                if (!success)
                {
                    await Clients.Group(gameId).SendAsync("GameOver", chessService.GetResults(gameId));
                    chessService.RemoveGame(gameId);
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
            if (!chessService.GameExists(gameId))
            {
                await Clients.Caller.SendAsync("Error", "Game not found.");
                return Array.Empty<Move>();
            }
            return chessService.GetPossibleMoves(gameId, pos);
        }

        public async Task PromotePawn(string gameId, PawnPromotion pawnPromotion, PieceType promotion)
        {
            if (!chessService.GameExists(gameId))
            {
                await Clients.Caller.SendAsync("Error", "Game not found.");
                return;
            }

            chessService.PromotePawn(gameId, pawnPromotion, promotion);
            await UpdateBoard(gameId);
        }
        
        private async Task UpdateBoard(string gameId)
        {
            if (!chessService.GameExists(gameId)) return;
            await Clients.Group(gameId).SendAsync("UpdateBoard", chessService.GetBoard(gameId));
        }
    }
}
