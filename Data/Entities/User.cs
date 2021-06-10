using System.Collections.Generic;

namespace TodoServer.Data.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string HashedPassword { get; set; }
        public List<TodoItem> Todos { get; set; }
    }
}