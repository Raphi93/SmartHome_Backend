using SmartHome_Backend_NoSQL.Models;

namespace SmartHome_Backend_NoSQL.Service
{
    public interface IWeatherAverage
    {
        public WeatherAverageModel Get(string dayTime);
        public void Add(WeatherAverageModel weather);
    }
}
