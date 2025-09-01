namespace TodoApp.Application.Common.DTOs;

public class CreateSubtaskRequest
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int Order { get; set; } = 0;
}