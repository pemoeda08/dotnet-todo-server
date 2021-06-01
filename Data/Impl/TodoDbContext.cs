using Microsoft.EntityFrameworkCore;

namespace TodoServer.Data.Impl
{
    public class TodoDbContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder
                .UseNpgsql("User ID=todoapp; Password=test1234;Host=localhost;Database=todoapp;")
                .EnableDetailedErrors()
                .EnableSensitiveDataLogging();
        }
    }
}