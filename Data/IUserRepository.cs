using System.Threading.Tasks;
using TodoServer.Data.Entities;

namespace TodoServer.Data
{
    public interface IUserRepository
    {
        Task<User> GetUser(string username, string hashedPassword);
    }
}