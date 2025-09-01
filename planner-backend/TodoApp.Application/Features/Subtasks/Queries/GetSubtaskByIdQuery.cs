using MediatR;
using TodoApp.Application.Common.DTOs;
using TodoApp.Application.Common.Interfaces;

namespace TodoApp.Application.Features.Subtasks.Queries;

public record GetSubtaskByIdQuery(int Id) : IRequest<SubtaskDto?>;

public class GetSubtaskByIdQueryHandler : IRequestHandler<GetSubtaskByIdQuery, SubtaskDto?>
{
    private readonly ISubtaskRepository _subtaskRepository;
    private readonly IAuthService _authService;

    public GetSubtaskByIdQueryHandler(ISubtaskRepository subtaskRepository, IAuthService authService)
    {
        _subtaskRepository = subtaskRepository;
        _authService = authService;
    }

    public async Task<SubtaskDto?> Handle(GetSubtaskByIdQuery request, CancellationToken cancellationToken)
    {
        var userId = _authService.GetCurrentUserId();
        
        // Verify that the subtask belongs to the current user through todo ownership
        var hasAccess = await _subtaskRepository.VerifySubtaskOwnershipAsync(request.Id, userId);
        if (!hasAccess)
            return null;

        var subtask = await _subtaskRepository.GetByIdAsync(request.Id);
        if (subtask == null || subtask.IsDeleted)
            return null;

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