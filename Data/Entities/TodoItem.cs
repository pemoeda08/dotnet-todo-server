using System;

namespace TodoServer.Data.Entities {

    public class TodoItem {
        public long Id { get; set; }
        public string Text { get; set; }
        public DateTime CreatedAt { get; set; }

        public int CreatedById { get; set; }
        public User CreatedBy { get; set; }
    }
}