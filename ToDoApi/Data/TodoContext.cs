using Microsoft.EntityFrameworkCore;
using TodoApi.Models;

namespace TodoApi.Data;

// It manages the connection to the database and provides access to the Todos table
public class TodoContext : DbContext
{
    public TodoContext(DbContextOptions<TodoContext> options)
        : base(options)
    {
    }

    public DbSet<Todo> Todos => Set<Todo>();
}
