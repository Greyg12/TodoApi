using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoApi.Controllers;
using TodoApi.Data;
using TodoApi.Models;
using Xunit;

namespace TodoApi.Tests;

public class TodoControllerTests
{
    // Set up the controller with an in-memory database
    private TodoController GetControllerWithContext()
    {
        var options = new DbContextOptionsBuilder<TodoContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        var context = new TodoContext(options);
        return new TodoController(context);
    }

    // Test: Should create a todo item and return it
    [Fact]
    public async Task CreateTodo_ShouldReturnCreatedTodo()
    {
        var controller = GetControllerWithContext();
        var todo = new Todo
        {
            Title = "Test Task",
            Expiry = DateTime.UtcNow.AddDays(1),
            PercentComplete = 0
        };

        var result = await controller.CreateTodo(todo);

        var actionResult = Assert.IsType<ActionResult<Todo>>(result);
        var createdAtAction = Assert.IsType<CreatedAtActionResult>(actionResult.Result);
        var returnedTodo = Assert.IsType<Todo>(createdAtAction.Value);
        Assert.Equal("Test Task", returnedTodo.Title);
    }

    // Should return empty list when no todos exist
    [Fact]
    public async Task GetAll_ShouldReturnEmpty_WhenNoTodos()
    {
        var controller = GetControllerWithContext();

        var result = await controller.GetAll();

        Assert.Empty(result.Value);
    }

    // Should set PercentComplete to 100 when marked as done
    [Fact]
    public async Task MarkAsDone_ShouldSetPercentTo100()
    {
        var controller = GetControllerWithContext();
        var context = new TodoContext(
            new DbContextOptionsBuilder<TodoContext>()
                .UseInMemoryDatabase("MarkAsDoneTest")
                .Options);

        var todo = new Todo
        {
            Title = "Complete this test",
            Expiry = DateTime.UtcNow.AddDays(2),
            PercentComplete = 20
        };

        context.Todos.Add(todo);
        context.SaveChanges();

        var controllerWithDb = new TodoController(context);

        await controllerWithDb.MarkAsDone(todo.Id);

        var updatedTodo = context.Todos.First(t => t.Id == todo.Id);
        Assert.Equal(100, updatedTodo.PercentComplete);
    }
}
