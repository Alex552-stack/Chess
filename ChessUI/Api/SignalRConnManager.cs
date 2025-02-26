using System.Threading.Tasks;
using ChessLogic;
using ChessLogic.Moves;
using Microsoft.AspNetCore.SignalR.Client;

namespace ChessUI.Api;

public class SignalRConnManager
{
    private readonly HubConnection _hubConnection;


    public SignalRConnManager(string hubUrl)
    {
        _hubConnection = new HubConnectionBuilder()
            .WithUrl(hubUrl)
            .Build();
    }

    public async Task StartConnectionAsync()
    {
        await _hubConnection.StartAsync();
    }

    public async Task<string> CreateGameAsync()
    {
        return await _hubConnection.InvokeAsync<string>("CreateGame");
    }

    public async Task MakeMoveAsync(string gameId, Move move)
    {
        await _hubConnection.InvokeAsync("MakeMove", gameId, move);
    }

    public async Task<Board> UpdateBoardAsync(string gameId)
    {
        return await _hubConnection.InvokeAsync<Board>("UpdateBoard", gameId);
    }
}