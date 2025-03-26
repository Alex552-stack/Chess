using ChessLogic;
using ChessLogic.Moves;

namespace ChessServer.Services;

public interface IChesService
{
    bool GameExists(string gameId);
    string[] StartGame();
    GameState GetGameState(string gameId);
    int GetPlayersCount(string gameId);
    void IncrementPlayersCount(string gameId);
    bool MakeMove(string gameId, Move move);
    Board GetBoard(string gameId);
    GameState? GetState(string gameId);
    ChessLogic.Results GetResults(string gameId);
    Move[] GetPossibleMoves(string gameId, Position position);
    void PromotePawn(string gameId, PawnPromotion pawnPromotion, PieceType promotion);
    void RemoveGame(string gameId);
}