using System;
using System.Windows;
using System.Windows.Controls;
using ChessLogic;
using ChessLogic.Pieces;

namespace ChessUI;

/// <summary>
///     Interaction logic for PawnPromotion.xaml
/// </summary>
public partial class PawnPromotionMenu : UserControl
{
    public PawnPromotionMenu(Piece piece)
    {
        Pawn = piece;
        InitializeComponent();
        SetImages();
    }

    private Piece Pawn { get; }


    public event Action<PieceType> OptionSelected;


    private void SetImages()
    {
        QueenImage.Source = new ImageParser().GetImage(Pawn.Color, PieceType.Queen);
        RookImage.Source = new ImageParser().GetImage(Pawn.Color, PieceType.Rook);
        BishopImage.Source = new ImageParser().GetImage(Pawn.Color, PieceType.Bishop);
        KnightImage.Source = new ImageParser().GetImage(Pawn.Color, PieceType.Knight);
    }

    private void Queen_Click(object sender, RoutedEventArgs e)
    {
        OptionSelected.Invoke(PieceType.Queen);
    }

    private void Rook_Click(object sender, RoutedEventArgs e)
    {
        OptionSelected.Invoke(PieceType.Rook);
    }

    private void Bishop_Click(object sender, RoutedEventArgs e)
    {
        OptionSelected.Invoke(PieceType.Bishop);
    }

    private void Knight_Click(object sender, RoutedEventArgs e)
    {
        OptionSelected.Invoke(PieceType.Knight);
    }
}