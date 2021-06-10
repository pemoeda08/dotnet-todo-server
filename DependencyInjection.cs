using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using TodoServer.Data;
using TodoServer.Data.Impl;

namespace TodoServer
{
    public static class ServiceExtensions
    {

        public static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<ITodoRepository, TodoRepository>();
            return services;
        }
    }
}