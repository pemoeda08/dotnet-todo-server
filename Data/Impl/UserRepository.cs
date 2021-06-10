using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TodoServer.Data.Entities;

namespace TodoServer.Data.Impl
{
    public class UserRepository : IUserRepository
    {
        private readonly TodoDbContext dbContext;

        public UserRepository(TodoDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<User> CreateUser(User newUser)
        {
            dbContext.Users.Add(newUser);
            await dbContext.SaveChangesAsync();
            return newUser;
        }

        public Task<bool> Exists(string username) => dbContext.Users.AnyAsync(x => string.Equals(x.Username, username));

        public Task<User> GetUser(string username, string hashedPassword) => 
            dbContext.Users
                .Where(x => x.Username == username && x.HashedPassword == hashedPassword)
                .FirstOrDefaultAsync();

        public async Task<User> GetUser(int userId)
        {
            var user = await dbContext.Users.FindAsync(userId);
            return user;
        }
    }
}