using System.ComponentModel.DataAnnotations;

namespace TodoApi.Models;

public class Todo
{
    public int Id { get; set; }

    [Required]
    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }

    [Required]
    public DateTime Expiry { get; set; }

    [Range(0, 100)]
    public int PercentComplete { get; set; } = 0;
}