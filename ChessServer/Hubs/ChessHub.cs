using ChessLogic;
using ChessLogic.Moves;
using ChessServer.Services;
using Microsoft.AspNetCore.SignalR;

namespace ChessServer.Hubs
{
	public class ChessHub : Hub
	{
		private readonly ChessService _chessService;
		public ChessHub(ChessService chessService)
		{
			_chessService = chessService;
		}

		public async Task CreateGame()
		{
			string connectionId = Context.ConnectionId;
			string[] gameData = _chessService.StartGame();
			
			// Add the player to the group associated with the game ID
			await Groups.AddToGroupAsync(connectionId, gameData[0]);
			

			// Notify players or send additional information about game start
			await Clients.Group(gameData[0]).SendAsync("GameStarted", gameData[0], _chessService.GetBoard(gameData[0]));
			await Clients.Caller.SendAsync("GetColor", gameData[1]);
		}
		public async Task MakeMove(string gameId, Move move)
		{
			// Perform game move logic
			if(!_chessService.MakeMove(gameId, move))
			{
				await Clients.Group(gameId).SendAsync("GameOver", _chessService.GetResults(gameId));
				_chessService.RemoveGame(gameId);
				await Clients.Group(gameId).SendAsync("Disconnect");

			}

			// Notify players about the updated board state or other relevant information
			await UpdateBoard(gameId);
		}

		public async Task Disconnect(string gameId)
		{
			await Groups.RemoveFromGroupAsync(Context.ConnectionId, gameId);
		}

		public async Task<Move[]> LegalMovesForPiece(string gameId, Position pos)
		{
			//await Clients.Caller.SendAsync("GetLegalMovesForPiece", _chessService.GetPossibleMoves(gameId, pos));
			return _chessService.GetPossibleMoves(gameId, pos);
		}

		public async Task PromotePawn(string gameId, PawnPromotion pawnPromotion, PieceType promotion)
		{
			await UpdateBoard(gameId);
			_chessService.PromotePawn(gameId, pawnPromotion, promotion);
			await UpdateBoard(gameId);
		}
		
		private async Task UpdateBoard(string gameId)
		{
			await Clients.Group(gameId).SendAsync("UpdateBoard", _chessService.GetBoard(gameId));
		}
	}
}
