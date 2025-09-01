using MediatR;
using TodoApp.Application.Common.DTOs;
using TodoApp.Application.Common.Interfaces;
using TodoApp.Domain.Entities;
using TodoApp.Domain.Enums;

namespace TodoApp.Application.Features.Todos.Commands;

public record CreateTodoCommand(CreateTodoRequest Request) : IRequest<TodoDto>;

public class CreateTodoCommandHandler : IRequestHandler<CreateTodoCommand, TodoDto>
{
    private readonly ITodoRepository _todoRepository;
    private readonly IAuthService _authService;

    public CreateTodoCommandHandler(ITodoRepository todoRepository, IAuthService authService)
    {
        _todoRepository = todoRepository;
        _authService = authService;
    }

    public async Task<TodoDto> Handle(CreateTodoCommand request, CancellationToken cancellationToken)
    {
        var userId = _authService.GetCurrentUserId();
        var now = DateTime.UtcNow;

        var todo = new Todo
        {
            Title = request.Request.Title,
            Description = request.Request.Description,
            Priority = request.Request.Priority,
            DueDate = request.Request.DueDate,
            Order = request.Request.Order,
            Status = TodoStatus.Pending,
            UserId = userId,
            CreatedAt = now,
            UpdatedAt = now
        };

        await _todoRepository.AddAsync(todo);

        return new TodoDto
        {
            Id = todo.Id,
            Title = todo.Title,
            Description = todo.Description,
            Status = todo.Status,
            Priority = todo.Priority,
            DueDate = todo.DueDate,
            Order = todo.Order,
            CreatedAt = todo.CreatedAt,
            UpdatedAt = todo.UpdatedAt,
            UserId = todo.UserId,
            Subtasks = new List<SubtaskDto>()
        };
    }
}