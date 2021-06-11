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
    [ApiController]
    public class TodoController : ControllerBase
    {
        private readonly ITodoRepository todoRepo;
        private readonly IMapper mapper;

        public TodoController(ITodoRepository todoRepo, IMapper mapper)
        {
            this.todoRepo = todoRepo;
            this.mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<List<TodoItemDto>>> GetOwnedTodos()
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
        public async Task<ActionResult<List<TodoItemDto>>> AddTodos([FromBody] IEnumerable<AddTodoItemDto> addTodoItems)
        {
            var userId = int.Parse(User.FindFirst(x => x.Type == "id")?.Value);

            var newTodos = addTodoItems.Select(nt => new TodoItem
            {
                CreatedById = userId,
                Text = nt.Text
            });
            
            newTodos  = await todoRepo.AddTodos(newTodos);
            var createdTodos = mapper.Map<List<TodoItemDto>>(newTodos);
            return Created(string.Empty, createdTodos);
        }

        [HttpDelete]
        public async Task<IActionResult> RemoveTodos([FromQuery, Required] long[] ids)
        {
            if (ids.Length == 0)
                return BadRequest(new {
                    title = "must specify at least one id."
                });
                
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