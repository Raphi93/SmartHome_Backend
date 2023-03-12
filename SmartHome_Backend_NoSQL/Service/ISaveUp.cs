using SmartHome_Backend_NoSQL.Models;

namespace SmartHome_Backend_NoSQL.Service
{
    public interface ISaveUp
    {
        public List<SaveUP> GetAll(string name);
        public void Add(SaveUP saveUp);
        public void Delete(string id);
        public void DeleteAll(string name);
    }
}
