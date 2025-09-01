using TodoApp.Domain.Common;
using TodoApp.Domain.Enums;

namespace TodoApp.Domain.Entities;

public class Subtask : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public TodoStatus Status { get; set; } = TodoStatus.Pending;
    public int Order { get; set; } = 0;
    public bool IsDeleted { get; set; } = false;
    
    public int TodoId { get; set; }
    public virtual Todo Todo { get; set; } = null!;
}