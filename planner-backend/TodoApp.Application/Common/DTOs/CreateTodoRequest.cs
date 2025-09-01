using TodoApp.Domain.Enums;

namespace TodoApp.Application.Common.DTOs;

public class CreateTodoRequest
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public Priority Priority { get; set; } = Priority.Medium;
    public DateTime? DueDate { get; set; }
}