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
using Swashbuckle.AspNetCore.Annotations;

namespace TodoServer
{

    [Route("todo")]
    [ApiController]
    [SwaggerTag("Manage todo(s)")]
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
        [Authorize]
        [SwaggerOperation( Summary = "Get logged user's owned todos")]
        [SwaggerResponse(200, "Returns list of todos owned by current user", typeof(List<TodoItemDto>))]
        public async Task<ActionResult<List<TodoItemDto>>> GetOwnedTodos()
        {
            var userId = int.Parse(User.FindFirst(x => x.Type == "id")?.Value);
            var todos = await todoRepo.GetUserTodos(userId);
            var todoDtos = mapper.Map<List<TodoItemDto>>(todos);
            return Ok(todoDtos);
        }

        [HttpGet("{id}")]
        [Authorize]
        [SwaggerOperation( Summary = "Get specified todo information")]
        [SwaggerResponse(200, "Returns todo information", typeof(TodoItemDto))]
        [SwaggerResponse(404, "If specified todo is not found")]
        [SwaggerResponse(403, "If the specified todo is not owned by current user")]
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
        [Authorize]
        [SwaggerOperation( Summary = "Add new todo(s)")]
        [SwaggerResponse(201, "Todo is created", typeof(List<TodoItemDto>))]
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
        [Authorize]
        [SwaggerOperation( Summary = "Remove todo(s)")]
        [SwaggerResponse(204, "Specified todo(s) successfully removed" )]
        [SwaggerResponse(400, "If there is not at least one id specified")]
        [SwaggerResponse(403, "If the user doesn't have access to the specified id(s)", typeof(int[]))]
        [SwaggerResponse(404, "If one or more of specified id is not found", typeof(int[]))]
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
        [Authorize]
        [SwaggerOperation( Summary = "Remove specified todo")]
        [SwaggerResponse(404, "If specified todo doesn't exist")]
        [SwaggerResponse(403, "If current user doesn't have access to the specified todo")]
        [SwaggerResponse(204, "Specified todo successfully removed")]
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