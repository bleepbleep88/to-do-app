using MediatR;
using TodoApp.Application.Common.DTOs;
using TodoApp.Application.Common.Interfaces;

namespace TodoApp.Application.Features.Subtasks.Commands;

public record UpdateSubtaskCommand(int Id, UpdateSubtaskRequest Request) : IRequest<SubtaskDto>;

public class UpdateSubtaskCommandHandler : IRequestHandler<UpdateSubtaskCommand, SubtaskDto>
{
    private readonly ISubtaskRepository _subtaskRepository;
    private readonly IAuthService _authService;

    public UpdateSubtaskCommandHandler(ISubtaskRepository subtaskRepository, IAuthService authService)
    {
        _subtaskRepository = subtaskRepository;
        _authService = authService;
    }

    public async Task<SubtaskDto> Handle(UpdateSubtaskCommand request, CancellationToken cancellationToken)
    {
        var userId = _authService.GetCurrentUserId();
        
        // Verify that the subtask belongs to the current user through todo ownership
        var hasAccess = await _subtaskRepository.VerifySubtaskOwnershipAsync(request.Id, userId);
        if (!hasAccess)
            throw new ArgumentException($"Subtask with id {request.Id} not found for current user");

        var subtask = await _subtaskRepository.GetByIdAsync(request.Id);
        if (subtask == null)
            throw new ArgumentException($"Subtask with id {request.Id} not found");

        subtask.Title = request.Request.Title;
        subtask.Description = request.Request.Description;
        subtask.Status = request.Request.Status;
        subtask.Order = request.Request.Order;
        subtask.UpdatedAt = DateTime.UtcNow;

        await _subtaskRepository.UpdateAsync(subtask);

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