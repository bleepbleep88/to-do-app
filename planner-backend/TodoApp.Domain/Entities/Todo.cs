using TodoApp.Domain.Common;
using TodoApp.Domain.Enums;

namespace TodoApp.Domain.Entities;

public class Todo : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public TodoStatus Status { get; set; } = TodoStatus.Pending;
    public Priority Priority { get; set; } = Priority.Medium;
    public DateTime? DueDate { get; set; }
    public bool IsDeleted { get; set; } = false;
    
    public int UserId { get; set; }
    public virtual User User { get; set; } = null!;
    public virtual ICollection<Subtask> Subtasks { get; set; } = new List<Subtask>();
}