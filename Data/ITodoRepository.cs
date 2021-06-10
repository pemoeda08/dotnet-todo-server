using System.Collections.Generic;
using System.Threading.Tasks;
using TodoServer.Data.Entities;

namespace TodoServer.Data
{

    public interface ITodoRepository
    {
        Task<List<TodoItem>> GetUserTodos(int userId);
        Task<TodoItem> GetTodo(long id);

        Task<TodoItem> AddTodo(TodoItem newItem);
        Task RemoveTodo(long id);

    }
}