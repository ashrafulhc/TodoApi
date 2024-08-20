using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoApi.Models;

namespace TodoApi.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class TodoItemsController : ControllerBase
    {
        private readonly UserContext _context;

        public TodoItemsController(UserContext context)
        {
            _context = context;
        }

        // GET: api/TodoItem
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TodoItemDto>>> GetTodoItems()
        {
            var username = User.Identity?.Name;
            if (username == null)
            {
                return Unauthorized();
            }

            // Include TodoItems when querying the user
            var user = await _context.Users
                .Include(u => u.TodoItems) // Load the related TodoItems
                .FirstOrDefaultAsync(u => u.Username.ToLower() == username.ToLower());

            if (user == null)
            {
                return NotFound("User not found");
            }

            var todoItems = user.TodoItems.Select(item => new TodoItemDto
            {
                Id = item.Id,
                Name = item.Name,
                IsComplete = item.IsComplete
            }).ToList();

            return Ok(todoItems);
        }

        // GET: api/TodoItems/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TodoItem>> GetTodoItem(long id)
        {
            var username = User.Identity?.Name;
            if (username == null)
            {
                return Unauthorized();
            }

            var user = await _context.Users
                .Include(u => u.TodoItems)
                .FirstOrDefaultAsync(u => u.Username.ToLower() == username.ToLower());

            if (user == null)
            {
                return NotFound("User not found");
            }

            var todoItem = user.TodoItems
                .FirstOrDefault(todo => todo.Id == id);

            if (todoItem == null)
            {
                return NotFound("Todo item not found");
            }

            var todoItemDto = new TodoItemDto
            {
                Id = todoItem.Id,
                Name = todoItem.Name,
                IsComplete = todoItem.IsComplete
            };

            return Ok(todoItemDto);
        }

        // PUT: api/TodoItems/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTodoItem(long id, TodoItemDto todoItemDto)
        {
            var username = User.Identity?.Name;
            if (username == null)
            {
                return Unauthorized();
            }

            var user = await _context.Users
                .Include(u => u.TodoItems)
                .FirstOrDefaultAsync(u => u.Username.ToLower() == username.ToLower());

            if (user == null)
            {
                return NotFound("User not found");
            }

            var todoItem = user.TodoItems.FirstOrDefault(item => item.Id == id);
            if (todoItem == null)
            {
                return NotFound("Todo item not found");
            }

            // Update the properties of the todoItem with the values from the DTO
            todoItem.Name = todoItemDto.Name;
            todoItem.IsComplete = todoItemDto.IsComplete;

            // Mark the entity as modified
            _context.Entry(todoItem).State = EntityState.Modified;

            // Save changes to the database
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // POST: api/TodoItems
        [HttpPost]
        public async Task<ActionResult<TodoItemDto>> PostTodoItem(TodoItemDto todoItemDto)
        {
            var username = User.Identity?.Name;
            if (username == null)
            {
                return Unauthorized();
            }
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Username.ToLower() == username.ToLower());

            if (user == null)
            {
                return NotFound("User not found");
            }

            var todoItem = new TodoItem
            {
                Name = todoItemDto.Name,
                IsComplete = todoItemDto.IsComplete,
                UserId = user.Id,
            };

            _context.TodoItems.Add(todoItem);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetTodoItem), new { id = todoItem.Id }, todoItemDto);
        }

        // DELETE: api/TodoItems/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTodoItem(long id)
        {

            var username = User.Identity?.Name;
            if (username == null)
            {
                return Unauthorized();
            }

            var user = await _context.Users
                .Include(u => u.TodoItems)
                .FirstOrDefaultAsync(u => u.Username.ToLower() == username.ToLower());

            if (user == null)
            {
                return NotFound("User not found");
            }

            var todoItem = user.TodoItems
                .FirstOrDefault(todo => todo.Id == id);

            if (todoItem == null)
            {
                return NotFound("Todo item not found");
            }

            _context.TodoItems.Remove(todoItem);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // GET: api/TodoItems/completed
        [HttpGet("completed")]
        public async Task<ActionResult<IEnumerable<TodoItem>>> GetCompletedItems()
        {
            var username = User.Identity?.Name;
            if (username == null)
            {
                return Unauthorized();
            }

            // Include TodoItems when querying the user
            var user = await _context.Users
                .Include(u => u.TodoItems) // Load the related TodoItems
                .FirstOrDefaultAsync(u => u.Username.ToLower() == username.ToLower());

            if (user == null)
            {
                return NotFound("User not found");
            }

            var todoItems = user.TodoItems
            .Where(todoItem => todoItem.IsComplete)
            .Select(todoItem => new TodoItemDto
            {
                Id = todoItem.Id,
                Name = todoItem.Name,
                IsComplete = todoItem.IsComplete
            }).ToList();

            return Ok(todoItems);
        }
    }
}
