using System.ComponentModel.DataAnnotations;

namespace TodoApp.Application.Common.DTOs;

public class ReorderTodosRequest
{
    [Required]
    public List<int> TodoIds { get; set; } = new();
}

public class ReorderSubtasksRequest
{
    [Required]
    public List<int> SubtaskIds { get; set; } = new();
}