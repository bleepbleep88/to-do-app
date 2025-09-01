using TodoApp.Domain.Entities;

namespace TodoApp.Application.Common.Interfaces;

public interface ISubtaskRepository : IRepository<Subtask>
{
    Task<IEnumerable<Subtask>> GetSubtasksByTodoIdAsync(int todoId);
    Task<Subtask?> GetSubtaskByIdAndTodoIdAsync(int id, int todoId);
    Task<bool> VerifySubtaskOwnershipAsync(int subtaskId, int userId);
    Task<List<Subtask>> GetSubtasksByIdsAsync(List<int> ids, int todoId);
    Task UpdateRangeAsync(List<Subtask> subtasks);
}