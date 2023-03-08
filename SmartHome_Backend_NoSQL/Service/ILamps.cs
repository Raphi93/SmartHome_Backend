using SmartHome_Backend_NoSQL.Models;

namespace SmartHome_Backend_NoSQL.Service
{
    public interface ILamps
    {
        public List<LichterModels> GetAll();
        public LichterModels Get(string name);
        public void Add(LichterModels lights);
        public void Update(string name, LichterModels lights);
    }
}
