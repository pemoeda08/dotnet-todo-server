using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TodoServer.Data.Entities;

namespace TodoServer.Data.Impl
{

    public class TodoRepository : ITodoRepository
    {
        private readonly TodoDbContext dbContext;

        public TodoRepository(TodoDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<TodoItem> AddTodo(TodoItem newItem)
        {
            dbContext.Add(newItem);
            await dbContext.SaveChangesAsync();
            return newItem;
        }

        public Task<TodoItem> GetTodo(long id) =>
            dbContext.Todos
                .Include(t => t.CreatedBy)
                .FirstOrDefaultAsync(t => t.Id == id);

        public Task<List<TodoItem>> GetTodos(params long[] ids) =>
            dbContext.Todos.Where(t => ids.Contains(t.Id)).ToListAsync();

        public Task<List<TodoItem>> GetUserTodos(int userId) =>
            dbContext.Todos
                .Where(t => t.CreatedById == userId)
                .Include(t => t.CreatedBy)
                .ToListAsync();

        public async Task RemoveTodo(long id)
        {
            var todo = await dbContext.Todos.FindAsync(id);
            if (todo == null)
                throw new KeyNotFoundException($"todo item with id of '{id}' not found.");
            dbContext.Todos.Remove(todo);
            await dbContext.SaveChangesAsync();
        }

        public async Task RemoveTodo(TodoItem todo)
        {
            dbContext.Todos.Remove(todo);
            await dbContext.SaveChangesAsync();
        }

        public async Task RemoveTodos(IEnumerable<TodoItem> todos)
        {
            dbContext.Todos.RemoveRange(todos);
            await dbContext.SaveChangesAsync();
        }

        public async Task RemoveTodos(params long[] ids)
        {
            var todos = await dbContext.Todos.Where(t => ids.Contains(t.Id)).ToListAsync();
            dbContext.RemoveRange(todos);
            await dbContext.SaveChangesAsync();
        }

        public async Task ClearTodo(int userId)
        {
            var todos = await dbContext.Todos
               .Where(t => t.CreatedById == userId)
               .ToListAsync();

            dbContext.Todos.RemoveRange(todos);
            await dbContext.SaveChangesAsync();
        }


    }
}