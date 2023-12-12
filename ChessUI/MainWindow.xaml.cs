using ChessLogic;
using ChessLogic.Moves;
using ChessUI.Network;
using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace ChessUI
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private readonly Image[,] pieceImages = new Image[8, 8];
		private readonly Rectangle[,] highLights = new Rectangle[8, 8];
		private readonly Dictionary<Position, Move> moveCache = new Dictionary<Position, Move>();
		private readonly ImageParser imageParser = new ImageParser();
		private DispatcherTimer timer1;
		private DispatcherTimer timer2;

		private TimeSpan time1 = TimeSpan.FromMinutes(5);
		private TimeSpan time2 = TimeSpan.FromMinutes(5);
		private Board Board;
		private Results Results;
		private Position selectedPos = null;
		private ServerConnection Server;
		private string GameId;
		private Player ThisPlayer;
		public MainWindow()
		{
			InitializeComponent();
			InitializeBoard();
			InitConn();
			SetCursor(ThisPlayer);
			//gameState = new GameState(Player.White, new Board());
		}

		private void InitializeBoard()
		{
			for (int i = 0; i < 8; i++)
			{
				for (int j = 0; j < 8; j++)
				{
					Image image = new Image();
					pieceImages[i, j] = image;
					PieceGrid.Children.Add(image);

					Rectangle highLight = new Rectangle();
					highLights[i, j] = highLight;
					HighLightGrid.Children.Add(highLight);
				}
			}
		}

		private void InitConn()
		{
			Server = new ServerConnection();


			Server.Conn.InvokeAsync("CreateGame");
			Server.Conn.On<string, Board>("GameStarted", (gameId, board) =>
			{
				Dispatcher.InvokeAsync(() =>
				{
					GameId = gameId;
					Board = board;
					DrawBoard();
				});

			});
			Server.Conn.On<string>("GetColor", (color) =>
			{
				Dispatcher.InvokeAsync(() =>
				{
					ThisPlayer = (Player)int.Parse(color);
				});
			});
			Server.Conn.On<Board>("UpdateBoard", (board) =>
			{
				Dispatcher.InvokeAsync(() =>
				{
					Board = board;
					DrawBoard();
				});
			});
			Server.Conn.On<Results>("GameOver", (results) =>
			{
				Dispatcher.InvokeAsync(() =>
				{
					Results = results;
					ShowGameOver();
				});
			});
			Server.Conn.On("Disconnect", () =>
			{
				Dispatcher.InvokeAsync(() =>
				{
					Server.Conn.InvokeAsync("Disconnect");
				});
			});

		}

		private void DrawBoard()
		{
			for (int i = 0; i < 8; i++)
			{
				for (int j = 0; j < 8; j++)
				{
					Piece piece = Board[i, j];
					pieceImages[i, j].Source = imageParser.GetImage(piece);
				}
			}
		}



		private void DrawBoard(Board board)
		{
			for (int i = 0; i < 8; i++)
			{
				for (int j = 0; j < 8; j++)
				{
					Piece piece = board[i, j];
					pieceImages[i, j].Source = imageParser.GetImage(piece);
				}
			}
		}

		private void BoardGrid_MouseDown(object sender, MouseButtonEventArgs e)
		{
			if (IsMenuOnScreen())
			{
				return;
			}

			Point point = e.GetPosition(BoardGrid);
			Position pos = ToSquarePosition(point);

			if (selectedPos == null)
			{
				OnFormPositionSelected(pos);
			}
			else
			{
				OnToFormPositionSelected(pos);
			}

		}

		private void OnToFormPositionSelected(Position pos)
		{
			selectedPos = null;
			HideHighlights();

			if (moveCache.TryGetValue(pos, out Move move))
			{
				HandleMove(move);
			}
		}

		private async void HandleMove(Move move)
		{

			//gameState.MakeMove(move);
			var gameId = GameId;
			await Server.Conn.InvokeAsync("MakeMove", GameId, move);
			if (move.Type == MoveType.PawnPromotion)
			{
				//DrawBoard(gameState.Board);
				/*PawnPromotion pawnPromotion = move as PawnPromotion;
				PieceType promotion = await ShowPawnPromotionAsync(gameState.Board[move.ToPos], pawnPromotion);
				gameState.PromotePawn(pawnPromotion, promotion);*/
				await Server.Conn.InvokeAsync("PromotePawn", gameId, move, await ShowPawnPromotionAsync(Board[move.ToPos], move as PawnPromotion));
			}
			
			//SwitchTimers();

			/*if (gameState.IsGameOver())
			{
				ShowGameOver();
			}*/
			//IsGameOver();
		}

		/*private void IsGameOver()
        {
            if (gameState.state == State.Playing)
            {
                string losingPlayerName = gameState.CurrentPlayer == Player.White ? "White" : "Black";
                winningMessageTextBlock.Text = $"{losingPlayerName} lost the game!"; // Set the winning message
                winningMessageTextBlock.Visibility = Visibility.Visible; // Make the TextBlock visible
            }
            else if (gameState.state == State.Draw) 
            {
                winningMessageTextBlock.Text = $"Draw"; // Set the winning message
                winningMessageTextBlock.Visibility = Visibility.Visible; // Make the TextBlock visible
            }
            

        }*/

		private async void OnFormPositionSelected(Position pos)
		{
			if (Board[pos] != null && Board[pos].Color == ThisPlayer)
			{
				Move[] moves = await Server.Conn.InvokeAsync<Move[]>("LegalMovesForPiece", GameId, pos);

				if (moves.Any())
				{
					selectedPos = pos;
					CacheMoves(moves);
					ShowHighlights();
				}
			}
			return;
		}

		private Position ToSquarePosition(Point point)
		{
			double squareSize = BoardGrid.ActualHeight / 8;
			int row = (int)((point.Y) / squareSize);
			int col = (int)((point.X) / squareSize);
			return new Position(row > 7 ? 7 : row, col > 7 ? 7 : col); //weird bug that appeared after adding the interface
		}

		private void CacheMoves(IEnumerable<Move> moves)
		{
			moveCache.Clear();

			foreach (Move move in moves)
			{
				moveCache[move.ToPos] = move;
			}
		}

		private void ShowHighlights()
		{
			Color color = Color.FromArgb(150, 125, 255, 125);

			foreach (Position to in moveCache.Keys)
			{
				highLights[to.Row, to.Column].Fill = new SolidColorBrush(color);
			}
		}

		private void HideHighlights()
		{
			foreach (Position to in moveCache.Keys)
			{
				highLights[to.Row, to.Column].Fill = Brushes.Transparent;
			}
		}

		private void SetCursor(Player player)
		{
			if (player == Player.White)
				Cursor = ChessCursors.WhiteCursor;
			else
				Cursor = ChessCursors.BlackCursor;
		}

		private bool IsMenuOnScreen()
		{
			return MenuContainer.Content != null;
		}

		private void ShowGameOver()
		{
			GameOverMenu gameOverMenu = new GameOverMenu(Results, ThisPlayer);
			MenuContainer.Content = gameOverMenu;
			gameOverMenu.OptionSelected += option =>
			{
				if (option == Option.Restart)
				{
					MenuContainer.Content = null;
					RestartGame();
				}
				else
				{
					Application.Current.Shutdown();
				}
			};
		}

		private async Task<PieceType> ShowPawnPromotionAsync(Piece piece, PawnPromotion pawnPromotion)
		{
			PawnPromotionMenu promoMenu = new PawnPromotionMenu(piece);
			MenuContainer.Content = promoMenu;

			TaskCompletionSource<PieceType> tcs = new TaskCompletionSource<PieceType>();

			promoMenu.OptionSelected += option =>
			{
				tcs.SetResult(option);
				MenuContainer.Content = null;
			};

			return await tcs.Task;
		}

		private void RestartGame()
		{
			HideHighlights();
			moveCache.Clear();
			//gameState = new GameState(Player.White, Board.Initial());
			DrawBoard(Board);
			SetCursor(ThisPlayer);
		}
	}


}


