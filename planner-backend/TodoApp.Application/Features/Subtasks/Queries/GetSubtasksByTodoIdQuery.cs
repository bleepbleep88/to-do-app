using MediatR;
using TodoApp.Application.Common.DTOs;
using TodoApp.Application.Common.Interfaces;

namespace TodoApp.Application.Features.Subtasks.Queries;

public record GetSubtasksByTodoIdQuery(int TodoId) : IRequest<IEnumerable<SubtaskDto>>;

public class GetSubtasksByTodoIdQueryHandler : IRequestHandler<GetSubtasksByTodoIdQuery, IEnumerable<SubtaskDto>>
{
    private readonly ISubtaskRepository _subtaskRepository;
    private readonly ITodoRepository _todoRepository;
    private readonly IAuthService _authService;

    public GetSubtasksByTodoIdQueryHandler(
        ISubtaskRepository subtaskRepository, 
        ITodoRepository todoRepository,
        IAuthService authService)
    {
        _subtaskRepository = subtaskRepository;
        _todoRepository = todoRepository;
        _authService = authService;
    }

    public async Task<IEnumerable<SubtaskDto>> Handle(GetSubtasksByTodoIdQuery request, CancellationToken cancellationToken)
    {
        var userId = _authService.GetCurrentUserId();
        
        // Verify that the todo belongs to the current user
        var todo = await _todoRepository.GetTodoByIdAndUserIdAsync(request.TodoId, userId);
        if (todo == null)
            throw new ArgumentException($"Todo with id {request.TodoId} not found for current user");

        var subtasks = await _subtaskRepository.GetSubtasksByTodoIdAsync(request.TodoId);

        return subtasks.Select(subtask => new SubtaskDto
        {
            Id = subtask.Id,
            Title = subtask.Title,
            Description = subtask.Description,
            Status = subtask.Status,
            Order = subtask.Order,
            CreatedAt = subtask.CreatedAt,
            UpdatedAt = subtask.UpdatedAt,
            TodoId = subtask.TodoId
        }).OrderBy(s => s.Order);
    }
}