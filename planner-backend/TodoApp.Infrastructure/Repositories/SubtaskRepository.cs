using Microsoft.EntityFrameworkCore;
using TodoApp.Application.Common.Interfaces;
using TodoApp.Domain.Entities;
using TodoApp.Infrastructure.Data;

namespace TodoApp.Infrastructure.Repositories;

public class SubtaskRepository : Repository<Subtask>, ISubtaskRepository
{
    public SubtaskRepository(TodoDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Subtask>> GetSubtasksByTodoIdAsync(int todoId)
    {
        return await _dbSet
            .Where(s => s.TodoId == todoId && !s.IsDeleted)
            .OrderBy(s => s.Order)
            .ThenBy(s => s.CreatedAt)
            .ToListAsync();
    }

    public async Task<Subtask?> GetSubtaskByIdAndTodoIdAsync(int id, int todoId)
    {
        return await _dbSet
            .FirstOrDefaultAsync(s => s.Id == id && s.TodoId == todoId && !s.IsDeleted);
    }

    public async Task<bool> VerifySubtaskOwnershipAsync(int subtaskId, int userId)
    {
        return await _dbSet
            .Include(s => s.Todo)
            .AnyAsync(s => s.Id == subtaskId && s.Todo.UserId == userId && !s.IsDeleted);
    }

    public async Task<List<Subtask>> GetSubtasksByIdsAsync(List<int> ids, int todoId)
    {
        return await _dbSet
            .Where(s => ids.Contains(s.Id) && s.TodoId == todoId && !s.IsDeleted)
            .ToListAsync();
    }

    public async Task UpdateRangeAsync(List<Subtask> subtasks)
    {
        _dbSet.UpdateRange(subtasks);
        await _context.SaveChangesAsync();
    }
}