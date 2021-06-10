using System.Threading.Tasks;
using TodoServer.Data.Entities;

namespace TodoServer.Data
{
    public interface IUserRepository
    {
        Task <bool> Exists(string username);
        Task<User> GetUser(string username, string hashedPassword);
        Task<User> GetUser(int userId);
        Task<User> CreateUser(User newUser);
    }
}