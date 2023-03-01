using SmartHome_Backend_NoSQL.Service;
using SmartHome_Backend_NoSQL.Models;
using Microsoft.OpenApi.Models;
using ApiKeyCustomAttributes.Attributes;
using System.Text;
using Microsoft.Extensions.Configuration;
using SmartHome_Backend_NoSQL.Atrributes;

public class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.Configure<WeatherStationDataBaseSetting>(
                  builder.Configuration.GetSection("WeatherStationDatabase"));

        builder.Services.AddScoped<IWeatherstation, WeatherstationMongoDB>();
        builder.Services.AddScoped<IWeatherAverage, WeatherAverageMongoDB>();
        builder.Services.AddScoped<ITempIndoor, TempIndoorMongoDB>();
        builder.Services.AddScoped<ITempIndoorAverage, TempIndoorAverageMongoDB>();
        builder.Services.AddScoped<IUser, User>();

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "ApiKey", Version = "v2" });
        });

        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();


        builder.Services.AddSwaggerGen();
        var app = builder.Build();

        // Generate new API key if needed
        // ApiKeyGenerator.GenerateNewApiKeyIfNeeded();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }
        app.UseHttpsRedirection();

        // Auth
        app.UseAuthentication();

        app.MapControllers();
        app.Run();
    }
}
