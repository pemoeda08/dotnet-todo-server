using System.Threading.Tasks;

namespace TodoServer.Data
{
    public interface IUserRepository
    {
        Task<User> GetUser(string username, string password);
    }

    public class User {
        public int Id { get; set; }
        public string Username { get; set; }
    }
}