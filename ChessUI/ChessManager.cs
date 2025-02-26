using System.Threading.Tasks;
using ChessLogic;
using ChessLogic.Moves;
using ChessUI.Api;

namespace ChessUI;

public class ChessManager
{
    private readonly SignalRConnManager _connectionManager;

    public ChessManager(SignalRConnManager connectionManager)
    {
        _connectionManager = connectionManager;
    }

    public async Task<string> StartGameAsync()
    {
        await _connectionManager.StartConnectionAsync();
        return await _connectionManager.CreateGameAsync();
    }

    public async Task MakeMoveAsync(string gameId, Move move)
    {
        await _connectionManager.MakeMoveAsync(gameId, move);
    }

    public async Task<Board> UpdateBoardAsync(string gameId)
    {
        return await _connectionManager.UpdateBoardAsync(gameId);
    }
}