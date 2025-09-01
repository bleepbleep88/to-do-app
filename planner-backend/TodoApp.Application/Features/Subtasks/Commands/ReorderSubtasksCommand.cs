using MediatR;
using TodoApp.Application.Common.DTOs;
using TodoApp.Application.Common.Interfaces;

namespace TodoApp.Application.Features.Subtasks.Commands;

public record ReorderSubtasksCommand(int UserId, int TodoId, ReorderSubtasksRequest Request) : IRequest<Unit>;

public class ReorderSubtasksCommandHandler : IRequestHandler<ReorderSubtasksCommand, Unit>
{
    private readonly ISubtaskRepository _subtaskRepository;
    private readonly ITodoRepository _todoRepository;

    public ReorderSubtasksCommandHandler(ISubtaskRepository subtaskRepository, ITodoRepository todoRepository)
    {
        _subtaskRepository = subtaskRepository;
        _todoRepository = todoRepository;
    }

    public async Task<Unit> Handle(ReorderSubtasksCommand request, CancellationToken cancellationToken)
    {
        // Verify the todo belongs to the user
        var todo = await _todoRepository.GetByIdAsync(request.TodoId);
        if (todo == null || todo.UserId != request.UserId)
            throw new ArgumentException("Todo not found or doesn't belong to user");

        // Get all subtasks and validate they belong to the todo
        var subtasks = await _subtaskRepository.GetSubtasksByIdsAsync(request.Request.SubtaskIds, request.TodoId);
        
        if (subtasks.Count != request.Request.SubtaskIds.Count)
            throw new ArgumentException("One or more subtasks not found or don't belong to todo");

        // Update the order based on the position in the array
        for (int i = 0; i < request.Request.SubtaskIds.Count; i++)
        {
            var subtask = subtasks.First(s => s.Id == request.Request.SubtaskIds[i]);
            subtask.Order = i;
            subtask.UpdatedAt = DateTime.UtcNow;
        }

        await _subtaskRepository.UpdateRangeAsync(subtasks);
        
        return Unit.Value;
    }
}