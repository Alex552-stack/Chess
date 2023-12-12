using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ChessUI.WPF_Components.Menus
{
	/// <summary>
	/// Interaction logic for StartMenu.xaml
	/// </summary>
	public partial class StartMenu : UserControl
	{
		public StartMenu()
		{
			InitializeComponent();
		}

		private void ConnectButton_Click(object sender, RoutedEventArgs e)
		{
			HubConnection connection = new HubConnectionBuilder()
				.WithUrl("https://localhost:7284/chesshub")
				.Build();
		} 
	}
}
