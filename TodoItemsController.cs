using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoApi.Models;
using TodoApi.Persistance;

namespace TodoApi
{
    [Route("api/[controller]")]
    [ApiController]
    public class TodoItemsController : Controller
    {
        private readonly TodoContext _context;

        public TodoItemsController(TodoContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TodoItem>>> GetTodoItems()
        {
            return await _context.TodoItems.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TodoItem>> GetTodoItem(long id)
        {
            var _todoItem = await _context.TodoItems.FindAsync(id);
            if (_todoItem == null)
            {
                return NotFound();
            }

            return _todoItem;
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateTodoItem(long id, TodoItem todoItem)
        {
            if (id != todoItem.Id)
            {
                return BadRequest();
            }

            var _todoItem = await _context.TodoItems.FindAsync(id);
            if (_todoItem == null)
            {
                return NotFound();
            }

            _todoItem.Name = todoItem.Name;
            _todoItem.IsComplete = todoItem.IsComplete;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return Conflict(ex);
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteTodoItem(long id)
        {
            var _todoItem = await _context.TodoItems.FindAsync(id);
            if (_todoItem == null)
            {
                return NotFound();
            }

            _context.TodoItems.Remove(_todoItem);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPost]
        public async Task<ActionResult<TodoItem>> CreateTodoItem(TodoItem todoItem)
        {
            _context.TodoItems.Add(todoItem);
            await _context.SaveChangesAsync();
            return CreatedAtAction(
                nameof(GetTodoItem),
                new { id = todoItem.Id },
                todoItem
            );
        }
    }
}