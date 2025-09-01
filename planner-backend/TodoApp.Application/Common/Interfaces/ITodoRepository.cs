using TodoApp.Domain.Entities;

namespace TodoApp.Application.Common.Interfaces;

public interface ITodoRepository : IRepository<Todo>
{
    Task<IEnumerable<Todo>> GetTodosByUserIdAsync(int userId);
    Task<IEnumerable<Todo>> GetActiveTodosByUserIdAsync(int userId);
    Task<Todo?> GetTodoByIdAndUserIdAsync(int id, int userId);
}