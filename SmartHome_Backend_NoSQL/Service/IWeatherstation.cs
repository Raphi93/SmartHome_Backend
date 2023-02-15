using SmartHome_Backend_NoSQL.Models;

namespace SmartHome_Backend_NoSQL.Service
{
    public interface IWeatherstation
    {
        public List<WeatherSationModel> GetAll();
        public WeatherSationModel Get(string daytime);
        public void Add(WeatherSationModel weather);
        public void Update(string daytime, WeatherSationModel weather);
    }
}
