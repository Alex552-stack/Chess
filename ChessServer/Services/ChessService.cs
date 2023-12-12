using ChessLogic;
using ChessLogic.Moves;

namespace ChessServer.Services
{
    public class ChessService
    {
        private Dictionary<String, GameState> ActiveGames = new Dictionary<string, GameState>();

        public string[] StartGame()
        {
            string gameIdW1Player = ActiveGames.FirstOrDefault(kv => kv.Value.PlayersCount == 1).Key;
            if(!string.IsNullOrEmpty(gameIdW1Player) )
            {
                ActiveGames[gameIdW1Player].PlayersCount = 2;
                return new string[] { gameIdW1Player, "2" };
            }		
			string gameId = Guid.NewGuid().ToString();

            Board board = Board.Initial();
            GameState newGame = new GameState(Player.White, board);
            ActiveGames.Add(gameId, newGame);


            return new string[] { gameId, "1" };
		}

        public GameState GetGameState(string gameId)
        {
            return ActiveGames[gameId];
        }

        public int GetPlayersCount(string gameId)
        {
            return ActiveGames[gameId].PlayersCount;
        }

        public void IncrementPlayersCount(string gameId)
        {
            ActiveGames[gameId].PlayersCount++;
        }

        public bool MakeMove(string gameId, Move move)
        {
            ActiveGames[gameId].MakeMove(move);
            if (ActiveGames[gameId].IsGameOver())
                return false;
            return true;
        }

        public Board GetBoard(string gameId)
        {
            return ActiveGames[gameId].Board;
        }

		public ChessLogic.Results GetResults(string gameId)
        {
            return ActiveGames[gameId].Results;
        }

        public Move[] GetPossibleMoves(string gameId, Position position)
        {
            return ActiveGames[gameId].LegalMovesForPiece(position).ToArray();
        }

        public void PromotePawn(string gameId, PawnPromotion pawnPromotion, PieceType promotion)
        {
            ActiveGames[gameId].PromotePawn(pawnPromotion, promotion);
        }

        public void RemoveGame(string gameId)
        {
            ActiveGames.Remove(gameId);
        }
    }
}
