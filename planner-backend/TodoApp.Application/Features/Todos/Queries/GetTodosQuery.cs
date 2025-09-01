using MediatR;
using TodoApp.Application.Common.DTOs;
using TodoApp.Application.Common.Interfaces;

namespace TodoApp.Application.Features.Todos.Queries;

public record GetTodosQuery : IRequest<IEnumerable<TodoDto>>;

public class GetTodosQueryHandler : IRequestHandler<GetTodosQuery, IEnumerable<TodoDto>>
{
    private readonly ITodoRepository _todoRepository;
    private readonly ISubtaskRepository _subtaskRepository;
    private readonly IAuthService _authService;

    public GetTodosQueryHandler(ITodoRepository todoRepository, ISubtaskRepository subtaskRepository, IAuthService authService)
    {
        _todoRepository = todoRepository;
        _subtaskRepository = subtaskRepository;
        _authService = authService;
    }

    public async Task<IEnumerable<TodoDto>> Handle(GetTodosQuery request, CancellationToken cancellationToken)
    {
        var userId = _authService.GetCurrentUserId();
        var todos = await _todoRepository.GetActiveTodosByUserIdAsync(userId);

        var todoDtos = new List<TodoDto>();
        
        foreach (var todo in todos)
        {
            var subtasks = await _subtaskRepository.GetSubtasksByTodoIdAsync(todo.Id);
            
            todoDtos.Add(new TodoDto
            {
                Id = todo.Id,
                Title = todo.Title,
                Description = todo.Description,
                Status = todo.Status,
                Priority = todo.Priority,
                DueDate = todo.DueDate,
                CreatedAt = todo.CreatedAt,
                UpdatedAt = todo.UpdatedAt,
                UserId = todo.UserId,
                Subtasks = subtasks.Select(s => new SubtaskDto
                {
                    Id = s.Id,
                    Title = s.Title,
                    Description = s.Description,
                    Status = s.Status,
                    Order = s.Order,
                    CreatedAt = s.CreatedAt,
                    UpdatedAt = s.UpdatedAt,
                    TodoId = s.TodoId
                }).ToList()
            });
        }

        return todoDtos;
    }
}