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

        public Task<User> GetUser(string username, string hashedPassword) => 
            dbContext.Users
                .Where(x => x.Username == username && x.HashedPassword == hashedPassword)
                .FirstOrDefaultAsync();

        
    }
}