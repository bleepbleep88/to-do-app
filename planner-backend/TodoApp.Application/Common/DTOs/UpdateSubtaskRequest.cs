using TodoApp.Domain.Enums;

namespace TodoApp.Application.Common.DTOs;

public class UpdateSubtaskRequest
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public TodoStatus Status { get; set; }
    public int Order { get; set; }
}