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
using ChessLogic;
using ChessLogic.Moves;
using ChessLogic.Pieces;
using ChessUI.Network;
using Microsoft.AspNetCore.SignalR.Client;

namespace ChessUI;

/// <summary>
///     Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private readonly Rectangle[,] _highLights = new Rectangle[8, 8];
    private readonly ImageParser _imageParser = new();
    private readonly Dictionary<Position, Move> _moveCache = new();
    private readonly Image[,] _pieceImages = new Image[8, 8];
    private Board _board;
    private string _gameId;
    private Results _results;
    private Position _selectedPos;
    private ServerConnection _server;
    private Player _thisPlayer;

    private TimeSpan _time1 = TimeSpan.FromMinutes(5);
    private TimeSpan _time2 = TimeSpan.FromMinutes(5);
    private DispatcherTimer _timer1;
    private DispatcherTimer _timer2;

    public MainWindow()
    {
        InitializeComponent();
        InitializeBoard();
        InitConn();
        SetCursor(_thisPlayer);
        //gameState = new GameState(Player.White, new Board());
    }

    private void InitializeBoard()
    {
        for (var i = 0; i < 8; i++)
        for (var j = 0; j < 8; j++)
        {
            var image = new Image();
            _pieceImages[i, j] = image;
            PieceGrid.Children.Add(image);

            var highLight = new Rectangle();
            _highLights[i, j] = highLight;
            HighLightGrid.Children.Add(highLight);
        }
    }

    private void InitConn()
    {
        _server = new ServerConnection();


        _server.Conn.InvokeAsync("CreateGame");
        _server.Conn.On<string, Board>("GameStarted", (gameId, board) =>
        {
            Dispatcher.InvokeAsync(() =>
            {
                _gameId = gameId;
                _board = board;
                DrawBoard();
            });
        });
        _server.Conn.On<string>("GetColor",
            color => { Dispatcher.InvokeAsync(() => { _thisPlayer = (Player)int.Parse(color); }); });
        _server.Conn.On<Board>("UpdateBoard", board =>
        {
            Dispatcher.InvokeAsync(() =>
            {
                _board = board;
                DrawBoard();
            });
        });
        _server.Conn.On<Results>("GameOver", results =>
        {
            Dispatcher.InvokeAsync(() =>
            {
                _results = results;
                ShowGameOver();
            });
        });
        _server.Conn.On("Disconnect",
            () => { Dispatcher.InvokeAsync(() => { _server.Conn.InvokeAsync("Disconnect"); }); });
    }

    private void DrawBoard()
    {
        for (var i = 0; i < 8; i++)
        for (var j = 0; j < 8; j++)
        {
            Piece piece = _board[i, j];
            _pieceImages[i, j].Source = _imageParser.GetImage(piece);
        }
    }


    private void DrawBoard(Board board)
    {
        for (var i = 0; i < 8; i++)
        for (var j = 0; j < 8; j++)
        {
            Piece piece = board[i, j];
            _pieceImages[i, j].Source = _imageParser.GetImage(piece);
        }
    }

    private void BoardGrid_MouseDown(object sender, MouseButtonEventArgs e)
    {
        if (IsMenuOnScreen()) return;

        var point = e.GetPosition(BoardGrid);
        var pos = ToSquarePosition(point);

        if (_selectedPos == null)
            OnFormPositionSelected(pos);
        else
            OnToFormPositionSelected(pos);
    }

    private void OnToFormPositionSelected(Position pos)
    {
        _selectedPos = null;
        HideHighlights();

        if (_moveCache.TryGetValue(pos, out var move)) HandleMove(move);
    }

    private async void HandleMove(Move move)
    {
        //gameState.MakeMove(move);
        var gameId = _gameId;
        await _server.Conn.InvokeAsync("MakeMove", _gameId, move);
        if (move.Type == MoveType.PawnPromotion)
            //DrawBoard(gameState.Board);
            /*PawnPromotion pawnPromotion = move as PawnPromotion;
            PieceType promotion = await ShowPawnPromotionAsync(gameState.Board[move.ToPos], pawnPromotion);
            gameState.PromotePawn(pawnPromotion, promotion);*/
            await _server.Conn.InvokeAsync("PromotePawn", gameId, move,
                await ShowPawnPromotionAsync(_board[move.ToPos], move as PawnPromotion));

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
        if (_board[pos] == null || _board[pos].Color != _thisPlayer) return;
        var moves = await _server.Conn.InvokeAsync<Move[]>("LegalMovesForPiece", _gameId, pos);

        if (!moves.Any()) return;
        _selectedPos = pos;
        CacheMoves(moves);
        ShowHighlights();
    }

    private Position ToSquarePosition(Point point)
    {
        var squareSize = BoardGrid.ActualHeight / 8;
        var row = (int)(point.Y / squareSize);
        var col = (int)(point.X / squareSize);
        return new Position(row > 7 ? 7 : row, col > 7 ? 7 : col); //weird bug that appeared after adding the interface
    }

    private void CacheMoves(IEnumerable<Move> moves)
    {
        _moveCache.Clear();

        foreach (var move in moves) _moveCache[move.ToPos] = move;
    }

    private void ShowHighlights()
    {
        var color = Color.FromArgb(150, 125, 255, 125);

        foreach (var to in _moveCache.Keys) _highLights[to.Row, to.Column].Fill = new SolidColorBrush(color);
    }

    private void HideHighlights()
    {
        foreach (var to in _moveCache.Keys) _highLights[to.Row, to.Column].Fill = Brushes.Transparent;
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
        var gameOverMenu = new GameOverMenu(_results, _thisPlayer);
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
        var promoMenu = new PawnPromotionMenu(piece);
        MenuContainer.Content = promoMenu;

        var tcs = new TaskCompletionSource<PieceType>();

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
        _moveCache.Clear();
        //gameState = new GameState(Player.White, Board.Initial());
        DrawBoard(_board);
        SetCursor(_thisPlayer);
    }
}