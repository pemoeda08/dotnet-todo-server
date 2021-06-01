using System.Threading.Tasks;
using TodoServer.Data.Entities;

namespace TodoServer.Data.Impl
{
    public class UserRepository : IUserRepository
    {
        public Task<User> GetUser(string username, string hashedPassword)
        {
            throw new System.NotImplementedException();
        }
    }
}