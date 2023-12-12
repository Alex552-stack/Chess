using ChessServer.Data;
using ChessServer.Hubs;
using ChessServer.Services;
using System.Text.Json;

namespace ChessServer
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var builder = WebApplication.CreateBuilder(args);

			// Add services to the container.
			builder.Services.AddRazorPages();
			builder.Services.AddServerSideBlazor();
			builder.Services.AddSingleton<ChessService>();
			builder.Services.AddSignalR()
			.AddJsonProtocol(options =>
			{
				//options.PayloadSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
				options.PayloadSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
				//options.PayloadSerializerOptions.Converters.Add(new BoardConverter());
				options.PayloadSerializerOptions.Converters.Add(new PieceArrayConverter());
				options.PayloadSerializerOptions.Converters.Add(new PieceConverter());
				options.PayloadSerializerOptions.Converters.Add(new MoveConverter());
				
			});
			builder.Services.AddSingleton<WeatherForecastService>();
			

			var app = builder.Build();

			// Configure the HTTP request pipeline.
			if (!app.Environment.IsDevelopment())
			{
				app.UseExceptionHandler("/Error");
				// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
				app.UseHsts();
			}
			
			//app.UseHttpRedirection();

			app.UseStaticFiles();

			app.UseRouting();
			
			app.MapHub<ChessHub>("/chesshub");
			app.MapBlazorHub();
			app.MapFallbackToPage("/_Host");

			app.Run();
		}
	}
}