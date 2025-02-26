using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text.Json;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;

namespace ChessUI.Network;

public class ServerConnection
{
    public HubConnection Conn;

    public ServerConnection()
    {
        string serverUrl;

        // Check if running on the local machine
        //if (Environment.MachineName.Equals("ALEXSLITTLEWARM", StringComparison.OrdinalIgnoreCase))
        //{
        // Use localhost when running on the local machine
        //	serverUrl = "https://localhost:7284/chesshub";
        //}
        //else
        //{
        // Use the local IP address when running from another machine on the same network
        serverUrl = "https://localhost:7284/chesshub";
        //}
        Conn = new HubConnectionBuilder()
            .WithUrl(serverUrl)
            .WithAutomaticReconnect()
            .AddJsonProtocol(options =>
            {
                //options.PayloadSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                options.PayloadSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                //options.PayloadSerializerOptions.Converters.Add(new BoardConverter());
                // options.PayloadSerializerOptions.Converters.Add(new PieceArrayConverter());
                // options.PayloadSerializerOptions.Converters.Add(new PieceConverter());
                // options.PayloadSerializerOptions.Converters.Add(new MoveConverter());
            })
            .Build();
        try
        {
            Conn.StartAsync();
        }
        catch (Exception)
        {
            Console.WriteLine("Nu s-a conectat");
        }
    }

    private static string GetLocalIpAddress()
    {
        var host = Dns.GetHostEntry(Dns.GetHostName());
        var ip = host.AddressList.FirstOrDefault(address => address.AddressFamily == AddressFamily.InterNetwork);
        return ip?.ToString() ?? "127.0.0.1";
    }
}