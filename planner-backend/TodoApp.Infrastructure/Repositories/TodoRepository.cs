using Microsoft.EntityFrameworkCore;
using TodoApp.Application.Common.Interfaces;
using TodoApp.Domain.Entities;
using TodoApp.Infrastructure.Data;

namespace TodoApp.Infrastructure.Repositories;

public class TodoRepository : Repository<Todo>, ITodoRepository
{
    public TodoRepository(TodoDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Todo>> GetTodosByUserIdAsync(int userId)
    {
        return await _dbSet
            .Where(t => t.UserId == userId)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Todo>> GetActiveTodosByUserIdAsync(int userId)
    {
        return await _dbSet
            .Where(t => t.UserId == userId && !t.IsDeleted)
            .OrderBy(t => t.Order)
            .ToListAsync();
    }

    public async Task<Todo?> GetTodoByIdAndUserIdAsync(int id, int userId)
    {
        return await _dbSet
            .FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);
    }

    public async Task<List<Todo>> GetTodosByIdsAsync(List<int> ids, int userId)
    {
        return await _dbSet
            .Where(t => ids.Contains(t.Id) && t.UserId == userId)
            .ToListAsync();
    }

    public async Task UpdateRangeAsync(List<Todo> todos)
    {
        _dbSet.UpdateRange(todos);
        await _context.SaveChangesAsync();
    }
}