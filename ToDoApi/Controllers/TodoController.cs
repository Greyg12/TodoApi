using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoApi.Data;
using TodoApi.Models;

namespace TodoApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TodoController : ControllerBase
{
    private readonly TodoContext _context;

    public TodoController(TodoContext context)
    {
        _context = context;
    }

    // Get all todo items
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Todo>>> GetAll()
    {
        return await _context.Todos.ToListAsync();
    }

    // Get a todo item by its ID
    [HttpGet("{id}")]
    public async Task<ActionResult<Todo>> GetById(int id)
    {
        var todo = await _context.Todos.FindAsync(id);
        return todo == null ? NotFound() : Ok(todo);
    }

    // Get all todos that expire within the next 7 days
    [HttpGet("incoming")]
    public async Task<ActionResult<IEnumerable<Todo>>> GetIncoming()
    {
        var now = DateTime.UtcNow;
        var endOfWeek = now.Date.AddDays(7);

        var incoming = await _context.Todos
            .Where(t => t.Expiry.Date >= now.Date && t.Expiry.Date <= endOfWeek)
            .ToListAsync();

        return Ok(incoming);
    }

    // Create a new todo item
    [HttpPost]
    public async Task<ActionResult<Todo>> CreateTodo(Todo todo)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        _context.Todos.Add(todo);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = todo.Id }, todo);
    }

    // Update an existing todo item
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateTodo(int id, Todo updatedTodo)
    {
        if (id != updatedTodo.Id)
            return BadRequest("Mismatched Todo ID.");

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        _context.Entry(updatedTodo).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!_context.Todos.Any(t => t.Id == id))
                return NotFound();

            throw;
        }

        return NoContent();
    }

    // Update the completion percentage of a todo item
    [HttpPatch("{id}/updatePercent")]
    public async Task<IActionResult> SetPercentComplete(int id, [FromBody] int percent)
    {
        if (percent < 0 || percent > 100)
            return BadRequest("Percent must be between 0 and 100.");

        var todo = await _context.Todos.FindAsync(id);
        if (todo == null)
            return NotFound();

        todo.PercentComplete = percent;
        await _context.SaveChangesAsync();

        return NoContent();
    }

    // Mark a todo item as fully complete (100%)
    [HttpPatch("{id}/done")]
    public async Task<IActionResult> MarkAsDone(int id)
    {
        var todo = await _context.Todos.FindAsync(id);
        if (todo == null)
            return NotFound();

        todo.PercentComplete = 100;
        await _context.SaveChangesAsync();

        return NoContent();
    }

    // Delete a todo item by its ID
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTodo(int id)
    {
        var todo = await _context.Todos.FindAsync(id);
        if (todo == null)
            return NotFound();

        _context.Todos.Remove(todo);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
