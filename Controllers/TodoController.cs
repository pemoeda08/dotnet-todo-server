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
using TodoServer.Data;

namespace TodoServer
{

    [Authorize]
    [Route("todo")]
    public class TodoController : ControllerBase
    {
        private readonly ITodoRepository todoRepo;
        private readonly IUserRepository userRepo;
        private readonly IMapper mapper;

        public TodoController(ITodoRepository todoRepo, IUserRepository userRepo, IMapper mapper)
        {
            this.todoRepo = todoRepo;
            this.userRepo = userRepo;
            this.mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<List<TodoItemDto>>> GetTodos()
        {
            var userId = int.Parse(User.FindFirst(x => x.Type == "id")?.Value);
            var todos = await todoRepo.GetUserTodos(userId);
            var todoDtos = mapper.Map<List<TodoItemDto>>(todos);
            return Ok(todoDtos);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TodoItemDto>> GetTodo([Required]int? id)
        {
            var userId = int.Parse(User.FindFirst(x => x.Type == "id")?.Value);
            var todo = await todoRepo.GetTodo(id.Value);
            if (todo == null)
                return NotFound();

            if (todo.CreatedById != userId)
                return Forbid();
            
            var todoDto = mapper.Map<TodoItemDto>(todo);
            return Ok(todoDto);
        }

        [HttpPut]
        public async Task<ActionResult<TodoItemDto>> AddTodoItem([FromBody] AddTodoItemDto addTodoItem)
        {
            var userId = int.Parse(User.FindFirst(x => x.Type == "id")?.Value);
            var user = await userRepo.GetUser(userId);
            var newTodoItem = new TodoItem
            {
                CreatedBy = user,
                Text = addTodoItem.Text
            };

            newTodoItem = await todoRepo.AddTodo(newTodoItem);
            var createdTodo = mapper.Map<TodoItemDto>(newTodoItem);
            return Created(string.Empty, createdTodo);
        }
    }
}