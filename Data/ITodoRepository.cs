using System.Collections.Generic;
using System.Threading.Tasks;
using TodoServer.Data.Entities;

namespace TodoServer.Data
{

    public interface ITodoRepository
    {
        Task<List<TodoItem>> GetUserTodos(int userId);
        Task<TodoItem> GetTodo(long id);
        Task<List<TodoItem>> GetTodos(params long[] ids);
        Task<TodoItem> AddTodo(TodoItem newItem);
        Task<List<TodoItem>> AddTodos(IEnumerable<TodoItem> newTodos);
        Task RemoveTodo(long id);
        Task RemoveTodo(TodoItem todo);
        Task RemoveTodos(params long[] ids);
        Task RemoveTodos(IEnumerable<TodoItem> todos);
        Task ClearTodo(int userId);

    }
}