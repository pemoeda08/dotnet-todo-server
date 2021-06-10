using System.ComponentModel.DataAnnotations;

namespace TodoServer.Dto.Todo {

    public class AddTodoItemDto {

        [Required]
        public string Text { get; set; }
    }
}