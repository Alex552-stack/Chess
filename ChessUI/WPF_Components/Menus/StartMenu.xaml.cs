using System.Windows;
using System.Windows.Controls;
using Microsoft.AspNetCore.SignalR.Client;

namespace ChessUI.WPF_Components.Menus;

/// <summary>
///     Interaction logic for StartMenu.xaml
/// </summary>
public partial class StartMenu : UserControl
{
    public StartMenu()
    {
        InitializeComponent();
    }

    private void ConnectButton_Click(object sender, RoutedEventArgs e)
    {
        var connection = new HubConnectionBuilder()
            .WithUrl("https://localhost:7284/chesshub")
            .Build();
    }
}