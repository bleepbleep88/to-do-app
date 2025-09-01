using TodoApp.Domain.Enums;

namespace TodoApp.Application.Common.DTOs;

public class TodoDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public TodoStatus Status { get; set; }
    public Priority Priority { get; set; }
    public DateTime? DueDate { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public int UserId { get; set; }
    public List<SubtaskDto> Subtasks { get; set; } = new();
}