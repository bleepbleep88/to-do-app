using MediatR;
using TodoApp.Application.Common.Interfaces;

namespace TodoApp.Application.Features.Todos.Commands;

public record DeleteTodoCommand(int Id) : IRequest<bool>;

public class DeleteTodoCommandHandler : IRequestHandler<DeleteTodoCommand, bool>
{
    private readonly ITodoRepository _todoRepository;
    private readonly IAuthService _authService;

    public DeleteTodoCommandHandler(ITodoRepository todoRepository, IAuthService authService)
    {
        _todoRepository = todoRepository;
        _authService = authService;
    }

    public async Task<bool> Handle(DeleteTodoCommand request, CancellationToken cancellationToken)
    {
        var userId = _authService.GetCurrentUserId();
        var todo = await _todoRepository.GetTodoByIdAndUserIdAsync(request.Id, userId);

        if (todo == null)
            return false;

        todo.IsDeleted = true;
        todo.UpdatedAt = DateTime.UtcNow;
        await _todoRepository.UpdateAsync(todo);

        return true;
    }
}