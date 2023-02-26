using Microsoft.AspNetCore.Mvc;
using SmartHome_Backend_NoSQL.Models;

namespace SmartHome_Backend_NoSQL.Service
{
    public interface IUser
    {
        public string Post(UserModel user, IConfiguration configuration);
    }
}
