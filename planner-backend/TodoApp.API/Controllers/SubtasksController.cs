using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TodoApp.Application.Common.DTOs;
using TodoApp.Application.Common.Interfaces;
using TodoApp.Application.Features.Subtasks.Commands;
using TodoApp.Application.Features.Subtasks.Queries;

namespace TodoApp.API.Controllers;

[ApiController]
[Route("api/todos/{todoId}/subtasks")]
[Authorize]
public class SubtasksController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<SubtasksController> _logger;
    private readonly IAuthService _authService;

    public SubtasksController(IMediator mediator, ILogger<SubtasksController> logger, IAuthService authService)
    {
        _mediator = mediator;
        _logger = logger;
        _authService = authService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<SubtaskDto>>> GetSubtasks(int todoId)
    {
        try
        {
            _logger.LogInformation("Getting subtasks for todo {TodoId}", todoId);
            var subtasks = await _mediator.Send(new GetSubtasksByTodoIdQuery(todoId));
            return Ok(subtasks);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Todo {TodoId} not found for current user", todoId);
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving subtasks for todo {TodoId}", todoId);
            return StatusCode(500, "An error occurred while retrieving subtasks");
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<SubtaskDto>> GetSubtask(int todoId, int id)
    {
        try
        {
            _logger.LogInformation("Getting subtask {SubtaskId} for todo {TodoId}", id, todoId);
            var subtask = await _mediator.Send(new GetSubtaskByIdQuery(id));
            
            if (subtask == null)
            {
                _logger.LogWarning("Subtask {SubtaskId} not found", id);
                return NotFound();
            }

            if (subtask.TodoId != todoId)
            {
                _logger.LogWarning("Subtask {SubtaskId} does not belong to todo {TodoId}", id, todoId);
                return NotFound();
            }

            return Ok(subtask);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving subtask {SubtaskId}", id);
            return StatusCode(500, "An error occurred while retrieving the subtask");
        }
    }

    [HttpPost]
    public async Task<ActionResult<SubtaskDto>> CreateSubtask(int todoId, CreateSubtaskRequest request)
    {
        try
        {
            _logger.LogInformation("Creating new subtask for todo {TodoId}: {SubtaskTitle}", todoId, request.Title);
            var subtask = await _mediator.Send(new CreateSubtaskCommand(todoId, request));
            return CreatedAtAction(nameof(GetSubtask), new { todoId = todoId, id = subtask.Id }, subtask);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Todo {TodoId} not found for current user", todoId);
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating subtask for todo {TodoId}", todoId);
            return StatusCode(500, "An error occurred while creating the subtask");
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<SubtaskDto>> UpdateSubtask(int todoId, int id, UpdateSubtaskRequest request)
    {
        try
        {
            _logger.LogInformation("Updating subtask {SubtaskId}", id);
            var subtask = await _mediator.Send(new UpdateSubtaskCommand(id, request));
            
            if (subtask.TodoId != todoId)
            {
                _logger.LogWarning("Subtask {SubtaskId} does not belong to todo {TodoId}", id, todoId);
                return NotFound();
            }

            return Ok(subtask);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Subtask {SubtaskId} not found for current user", id);
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating subtask {SubtaskId}", id);
            return StatusCode(500, "An error occurred while updating the subtask");
        }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteSubtask(int todoId, int id)
    {
        try
        {
            _logger.LogInformation("Deleting subtask {SubtaskId}", id);
            var result = await _mediator.Send(new DeleteSubtaskCommand(id));
            
            if (!result)
            {
                _logger.LogWarning("Subtask {SubtaskId} not found for deletion", id);
                return NotFound();
            }

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting subtask {SubtaskId}", id);
            return StatusCode(500, "An error occurred while deleting the subtask");
        }
    }

    [HttpPost("reorder")]
    public async Task<ActionResult> ReorderSubtasks(int todoId, [FromBody] ReorderSubtasksRequest request)
    {
        try
        {
            var userId = _authService.GetCurrentUserId();
            _logger.LogInformation("Reordering {Count} subtasks for todo {TodoId} and user {UserId}", request.SubtaskIds.Count, todoId, userId);
            
            await _mediator.Send(new ReorderSubtasksCommand(userId, todoId, request));
            return Ok();
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Invalid subtask reorder request");
            return BadRequest(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error reordering subtasks for todo {TodoId}", todoId);
            return StatusCode(500, new { error = "An error occurred while reordering subtasks" });
        }
    }
}