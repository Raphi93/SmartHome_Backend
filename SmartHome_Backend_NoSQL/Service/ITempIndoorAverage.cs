using SmartHome_Backend_NoSQL.Models;

namespace SmartHome_Backend_NoSQL.Service
{
    public interface ITempIndoorAverage
    {
        public IndoorTempAveregaModel Get(string dayTime);
        public void Add(IndoorTempAveregaModel weather);
    }
}
