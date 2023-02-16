using SmartHome_Backend_NoSQL.Models;

namespace SmartHome_Backend_NoSQL.Service
{
    public interface ITempIndoor
    {
        public List<IndoorTempModel> GetAll();
        public IndoorTempModel Get(string dayTime);
        public void Add(IndoorTempModel weather);
        public void Update(string dayTime, IndoorTempModel weather);
    }
}
