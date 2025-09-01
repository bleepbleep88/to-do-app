using MediatR;
using TodoApp.Application.Common.DTOs;
using TodoApp.Application.Common.Interfaces;

namespace TodoApp.Application.Features.Todos.Commands;

public record UpdateTodoCommand(int Id, UpdateTodoRequest Request) : IRequest<TodoDto>;

public class UpdateTodoCommandHandler : IRequestHandler<UpdateTodoCommand, TodoDto>
{
    private readonly ITodoRepository _todoRepository;
    private readonly IAuthService _authService;

    public UpdateTodoCommandHandler(ITodoRepository todoRepository, IAuthService authService)
    {
        _todoRepository = todoRepository;
        _authService = authService;
    }

    public async Task<TodoDto> Handle(UpdateTodoCommand request, CancellationToken cancellationToken)
    {
        var userId = _authService.GetCurrentUserId();
        var todo = await _todoRepository.GetTodoByIdAndUserIdAsync(request.Id, userId);

        if (todo == null)
            throw new ArgumentException($"Todo with id {request.Id} not found for current user");

        todo.Title = request.Request.Title;
        todo.Description = request.Request.Description;
        todo.Status = request.Request.Status;
        todo.Priority = request.Request.Priority;
        todo.DueDate = request.Request.DueDate;
        todo.UpdatedAt = DateTime.UtcNow;

        await _todoRepository.UpdateAsync(todo);

        return new TodoDto
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
            Subtasks = new List<SubtaskDto>()
        };
    }
}