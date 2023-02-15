using Serilog;
using SmartHome_Backend_NoSQL.Controllers;
using SmartHome_Backend_NoSQL.Service;
using SmartHome_Backend_NoSQL.Models;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.Configure<WeatherStationDataBaseSetting>(
                  builder.Configuration.GetSection("WeatherStationDatabase"));

        builder.Services.AddScoped<IWeatherstation, WeatherstationMongoDB>();

        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}