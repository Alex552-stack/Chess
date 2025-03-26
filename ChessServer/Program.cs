using ChessServer.Hubs;
using ChessServer.Services;

namespace ChessServer;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        
        builder.Logging.ClearProviders();
        builder.Logging.AddConsole();

        builder.Services.AddControllers();
        
        builder.Services.AddSingleton<IChesService,ChesService>();
        builder.Services.AddSingleton<IMoveValidatorService,MoveValidatorService>();
        
        builder.Services.AddOpenApi();
        builder.Services.AddSwaggerGen();
        builder.Services.AddSignalR();
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowAll",
                policy =>
                {
                    policy.WithOrigins("http://localhost:3000")
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials();
                });
        });

        var app = builder.Build();
        // if (app.Environment.IsDevelopment())
        // {
            app.UseCors("AllowAll");
        // }

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        //app.UseHttpsRedirection();

        app.UseAuthorization();

        app.MapHub<ChessHub>("/chesshub");
        app.MapControllers();

        app.Run();
    }
}