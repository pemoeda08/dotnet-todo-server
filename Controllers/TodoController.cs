using System.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoServer.Data.Entities;
using TodoServer.Data.Impl;
using TodoServer.Dto.Todo;

namespace TodoServer
{

    [Authorize]
    [Route("todo")]
    public class TodoController : ControllerBase
    {
        private readonly TodoDbContext dbContext;
        private readonly IMapper mapper;

        public TodoController(TodoDbContext dbContext, IMapper mapper)
        {
            this.dbContext = dbContext;
            this.mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<List<TodoItemDto>>> GetTodos()
        {
            var userId = int.Parse(User.FindFirst(x => x.Type == "id")?.Value);
            var todos = await dbContext.Todos.Where(t => t.CreatedById == userId).ToListAsync();
            var todoDtos = mapper.Map<List<TodoItemDto>>(todos);
            return Ok(todoDtos);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TodoItemDto>> GetTodo([Required]int? id)
        {
            var userId = int.Parse(User.FindFirst(x => x.Type == "id")?.Value);
            var todo = await dbContext.Todos.FindAsync(id.Value);
            if (todo.CreatedById != userId)
                return Forbid();
            
            var todoDto = mapper.Map<TodoItemDto>(todo);
            return Ok(todoDto);
        }

        [HttpPut]
        public async Task<ActionResult<TodoItemDto>> AddTodoItem([FromBody] AddTodoItemDto addTodoItem)
        {
            var userId = int.Parse(User.FindFirst(x => x.Type == "id")?.Value);
            var user = await dbContext.Users.FindAsync(userId);
            var newTodoItem = new TodoItem
            {
                CreatedBy = user,
                Text = addTodoItem.Text
            };
            dbContext.Todos.Add(newTodoItem);
            await dbContext.SaveChangesAsync();
            var createdTodo = mapper.Map<TodoItemDto>(newTodoItem);
            return Created(string.Empty, createdTodo);
        }
    }
}