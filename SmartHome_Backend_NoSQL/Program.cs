using SmartHome_Backend_NoSQL.Service;
using SmartHome_Backend_NoSQL.Models;
using Microsoft.OpenApi.Models;

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
        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "JWT", Version = "v1" });
        });
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