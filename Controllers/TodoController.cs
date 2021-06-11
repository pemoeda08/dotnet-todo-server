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
using Microsoft.AspNetCore.Http;

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
        public async Task<ActionResult<TodoItemDto>> GetTodo([Required] int? id)
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

        [HttpPost]
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

        [HttpDelete]
        public async Task<IActionResult> RemoveTodos([FromQuery] long[] ids)
        {
            var todos = await todoRepo.GetTodos(ids);
            var idsNotFound = ids.Except(todos.Select(t => t.Id));
            if (idsNotFound.Any())
                return NotFound(new
                {
                    title = "specified resource(s) not found.",
                    detail = idsNotFound
                });

            var userId = int.Parse(User.FindFirst(x => x.Type == "id")?.Value);
            var unauthorizedTodos = todos.Where(t => t.CreatedById != userId);
            if (unauthorizedTodos.Any())
                return StatusCode(
                    statusCode: StatusCodes.Status403Forbidden,
                    value: new
                    {
                        title = "Unauthorized access to specified resource(s).",
                        detail = unauthorizedTodos
                    });

            await todoRepo.RemoveTodos(todos);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> RemoveTodo(long id)
        {
            var userId = int.Parse(User.FindFirst(x => x.Type == "id")?.Value);
            var todo = await todoRepo.GetTodo(id);
            if (todo == null)
                return Problem(
                    title: $"todo with id of '{id}' not found or has been deleted.",
                    statusCode: StatusCodes.Status404NotFound
                );

            if (todo.CreatedById != userId)
                return Forbid();

            await todoRepo.RemoveTodo(todo);
            return NoContent();
        }

    }
}