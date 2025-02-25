using ChessLogic;
using ChessLogic.Moves;

namespace ChessServer.Services
{
    public class ChessService
    {
        private readonly Dictionary<String, GameState> _activeGames = new Dictionary<string, GameState>();

        public string[] StartGame()
        {
            var gameIdW1Player = _activeGames.FirstOrDefault(kv => kv.Value.PlayersCount == 1).Key;
            if(!string.IsNullOrEmpty(gameIdW1Player) )
            {
                _activeGames[gameIdW1Player].PlayersCount = 2;
                return [gameIdW1Player, "2"];
            }		
			var gameId = Guid.NewGuid().ToString();

            var board = Board.Initial();
            var newGame = new GameState(Player.White, board);
            _activeGames.Add(gameId, newGame);


            return [gameId, "1"];
		}

        public GameState GetGameState(string gameId)
        {
            return _activeGames[gameId];
        }

        public int GetPlayersCount(string gameId)
        {
            return _activeGames[gameId].PlayersCount;
        }

        public void IncrementPlayersCount(string gameId)
        {
            _activeGames[gameId].PlayersCount++;
        }

        public bool MakeMove(string gameId, Move move)
        {
            _activeGames[gameId].MakeMove(move);
            if (_activeGames[gameId].IsGameOver())
                return false;
            return true;
        }

        public Board GetBoard(string gameId)
        {
            return _activeGames[gameId].Board;
        }

		public ChessLogic.Results GetResults(string gameId)
        {
            return _activeGames[gameId].Results;
        }

        public Move[] GetPossibleMoves(string gameId, Position position)
        {
            return _activeGames[gameId].LegalMovesForPiece(position).ToArray();
        }

        public void PromotePawn(string gameId, PawnPromotion pawnPromotion, PieceType promotion)
        {
            _activeGames[gameId].PromotePawn(pawnPromotion, promotion);
        }

        public void RemoveGame(string gameId)
        {
            _activeGames.Remove(gameId);
        }
    }
}
