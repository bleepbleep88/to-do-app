using TodoApp.Domain.Enums;

namespace TodoApp.Application.Common.DTOs;

public class SubtaskDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public TodoStatus Status { get; set; }
    public int Order { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public int TodoId { get; set; }
}