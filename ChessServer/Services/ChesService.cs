using ChessLogic;
using ChessLogic.Moves;

namespace ChessServer.Services;

public class ChesService
{
    private readonly Dictionary<string, GameState> _activeGames = new();

        public bool GameExists(string gameId) => _activeGames.ContainsKey(gameId);

        public string[] StartGame()
        {
            var availableGame = _activeGames.FirstOrDefault(kv => kv.Value.PlayersCount == 1).Key;
            if (!string.IsNullOrEmpty(availableGame))
            {
                _activeGames[availableGame].PlayersCount = 2;
                return [availableGame, "2"];
            }

            var gameId = Guid.NewGuid().ToString();
            _activeGames[gameId] = new GameState(Player.White, Board.Initial());

            return [gameId, "1"];
        }

        public GameState GetGameState(string gameId) => _activeGames.GetValueOrDefault(gameId);

        public int GetPlayersCount(string gameId) => _activeGames.GetValueOrDefault(gameId)?.PlayersCount ?? 0;

        public void IncrementPlayersCount(string gameId)
        {
            if (GameExists(gameId)) _activeGames[gameId].PlayersCount++;
        }

        public bool MakeMove(string gameId, Move move)
        {
            if (!GameExists(gameId)) return false;

            var game = _activeGames[gameId];
            game.MakeMove(move);
            return !game.IsGameOver();
        }

        public Board GetBoard(string gameId) => GameExists(gameId) ? _activeGames[gameId].Board : null;

        public ChessLogic.Results GetResults(string gameId) => GameExists(gameId) ? _activeGames[gameId].Results : null;

        public Move[] GetPossibleMoves(string gameId, Position position)
        {
            return GameExists(gameId) ? _activeGames[gameId].LegalMovesForPiece(position).ToArray() : Array.Empty<Move>();
        }

        public void PromotePawn(string gameId, PawnPromotion pawnPromotion, PieceType promotion)
        {
            if (GameExists(gameId)) _activeGames[gameId].PromotePawn(pawnPromotion, promotion);
        }

        public void RemoveGame(string gameId)
        {
            _activeGames.Remove(gameId);
        }
}