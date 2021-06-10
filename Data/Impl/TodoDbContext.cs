using System;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using TodoServer.Data.Entities;

namespace TodoServer.Data.Impl
{
    public class TodoDbContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder
                .UseNpgsql("User ID=todoapp; Password=test1234;Host=localhost;Database=todoapp;", opt => {
                    opt.SetPostgresVersion(new Version(12, 4));
                })
                .EnableDetailedErrors()
                .EnableSensitiveDataLogging();
        }

        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
    }
}