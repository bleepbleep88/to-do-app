using MediatR;
using TodoApp.Application.Common.DTOs;
using TodoApp.Application.Common.Interfaces;
using TodoApp.Domain.Entities;
using TodoApp.Domain.Enums;

namespace TodoApp.Application.Features.Subtasks.Commands;

public record CreateSubtaskCommand(int TodoId, CreateSubtaskRequest Request) : IRequest<SubtaskDto>;

public class CreateSubtaskCommandHandler : IRequestHandler<CreateSubtaskCommand, SubtaskDto>
{
    private readonly ISubtaskRepository _subtaskRepository;
    private readonly ITodoRepository _todoRepository;
    private readonly IAuthService _authService;

    public CreateSubtaskCommandHandler(
        ISubtaskRepository subtaskRepository, 
        ITodoRepository todoRepository,
        IAuthService authService)
    {
        _subtaskRepository = subtaskRepository;
        _todoRepository = todoRepository;
        _authService = authService;
    }

    public async Task<SubtaskDto> Handle(CreateSubtaskCommand request, CancellationToken cancellationToken)
    {
        var userId = _authService.GetCurrentUserId();
        
        // Verify that the todo belongs to the current user
        var todo = await _todoRepository.GetTodoByIdAndUserIdAsync(request.TodoId, userId);
        if (todo == null)
            throw new ArgumentException($"Todo with id {request.TodoId} not found for current user");

        var now = DateTime.UtcNow;
        var subtask = new Subtask
        {
            Title = request.Request.Title,
            Description = request.Request.Description,
            Order = request.Request.Order,
            Status = TodoStatus.Pending,
            TodoId = request.TodoId,
            CreatedAt = now,
            UpdatedAt = now
        };

        await _subtaskRepository.AddAsync(subtask);

        return new SubtaskDto
        {
            Id = subtask.Id,
            Title = subtask.Title,
            Description = subtask.Description,
            Status = subtask.Status,
            Order = subtask.Order,
            CreatedAt = subtask.CreatedAt,
            UpdatedAt = subtask.UpdatedAt,
            TodoId = subtask.TodoId
        };
    }
}