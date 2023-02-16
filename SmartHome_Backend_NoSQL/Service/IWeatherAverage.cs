using SmartHome_Backend_NoSQL.Models;

namespace SmartHome_Backend_NoSQL.Service
{
    public interface IWeatherAverage
    {
        public WeatherAverageModel Get(string _id);
        public void Add(WeatherAverageModel weather);
    }
}
