using MediatR;
using TodoApp.Application.Common.DTOs;
using TodoApp.Application.Common.Interfaces;

namespace TodoApp.Application.Features.Todos.Commands;

public record ReorderTodosCommand(int UserId, ReorderTodosRequest Request) : IRequest<Unit>;

public class ReorderTodosCommandHandler : IRequestHandler<ReorderTodosCommand, Unit>
{
    private readonly ITodoRepository _todoRepository;

    public ReorderTodosCommandHandler(ITodoRepository todoRepository)
    {
        _todoRepository = todoRepository;
    }

    public async Task<Unit> Handle(ReorderTodosCommand request, CancellationToken cancellationToken)
    {
        // Validate that all todos belong to the user
        var todos = await _todoRepository.GetTodosByIdsAsync(request.Request.TodoIds, request.UserId);
        
        if (todos.Count != request.Request.TodoIds.Count)
            throw new ArgumentException("One or more todos not found or don't belong to user");

        // Update the order based on the position in the array
        for (int i = 0; i < request.Request.TodoIds.Count; i++)
        {
            var todo = todos.First(t => t.Id == request.Request.TodoIds[i]);
            todo.Order = i;
            todo.UpdatedAt = DateTime.UtcNow;
        }

        await _todoRepository.UpdateRangeAsync(todos);
        
        return Unit.Value;
    }
}