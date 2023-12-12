using ChessLogic;
using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Threading.Tasks;

namespace ChessUI.Api
{
	public class SignalRConnManager
	{
		private readonly HubConnection hubConnection;


		public SignalRConnManager(string hubUrl)
		{
			hubConnection = new HubConnectionBuilder()
				.WithUrl(hubUrl)
				.Build();
		}

		public async Task StartConnectionAsync()
		{
			await hubConnection.StartAsync();
		}

		public async Task<string> CreateGameAsync()
		{
			return await hubConnection.InvokeAsync<string>("CreateGame");
		}

		public async Task MakeMoveAsync(string gameId, Move move)
		{
			await hubConnection.InvokeAsync("MakeMove", gameId, move);
		}

		public async Task<Board> UpdateBoardAsync(string gameId)
		{
			return await hubConnection.InvokeAsync<Board>("UpdateBoard", gameId);
		}
	}
}
