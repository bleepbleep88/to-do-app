using MediatR;
using TodoApp.Application.Common.Interfaces;

namespace TodoApp.Application.Features.Subtasks.Commands;

public record DeleteSubtaskCommand(int Id) : IRequest<bool>;

public class DeleteSubtaskCommandHandler : IRequestHandler<DeleteSubtaskCommand, bool>
{
    private readonly ISubtaskRepository _subtaskRepository;
    private readonly IAuthService _authService;

    public DeleteSubtaskCommandHandler(ISubtaskRepository subtaskRepository, IAuthService authService)
    {
        _subtaskRepository = subtaskRepository;
        _authService = authService;
    }

    public async Task<bool> Handle(DeleteSubtaskCommand request, CancellationToken cancellationToken)
    {
        var userId = _authService.GetCurrentUserId();
        
        // Verify that the subtask belongs to the current user through todo ownership
        var hasAccess = await _subtaskRepository.VerifySubtaskOwnershipAsync(request.Id, userId);
        if (!hasAccess)
            return false;

        var subtask = await _subtaskRepository.GetByIdAsync(request.Id);
        if (subtask == null)
            return false;

        subtask.IsDeleted = true;
        subtask.UpdatedAt = DateTime.UtcNow;
        await _subtaskRepository.UpdateAsync(subtask);

        return true;
    }
}