using SmartHome_Backend_NoSQL.Models;

namespace SmartHome_Backend_NoSQL.Service
{
    public interface ITempIndoorAverage
    {
        public IndoorTempAverageModel Get(string dayTime);
        public void Add(IndoorTempAverageModel weather);
    }
}
