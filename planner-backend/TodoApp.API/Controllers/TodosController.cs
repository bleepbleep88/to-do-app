using MediatR;
using Microsoft.AspNetCore.Mvc;
using TodoApp.Application.Common.DTOs;
using TodoApp.Application.Features.Todos.Commands;
using TodoApp.Application.Features.Todos.Queries;

namespace PlannerAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TodosController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<TodosController> _logger;

    public TodosController(IMediator mediator, ILogger<TodosController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<TodoDto>>> GetTodos()
    {
        try
        {
            _logger.LogInformation("Getting all todos for current user");
            var todos = await _mediator.Send(new GetTodosQuery());
            return Ok(todos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving todos");
            return StatusCode(500, "An error occurred while retrieving todos");
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<TodoDto>> GetTodo(int id)
    {
        try
        {
            _logger.LogInformation("Getting todo with id {TodoId}", id);
            var todo = await _mediator.Send(new GetTodoByIdQuery(id));
            
            if (todo == null)
            {
                _logger.LogWarning("Todo with id {TodoId} not found", id);
                return NotFound();
            }

            return Ok(todo);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving todo with id {TodoId}", id);
            return StatusCode(500, "An error occurred while retrieving the todo");
        }
    }

    [HttpPost]
    public async Task<ActionResult<TodoDto>> CreateTodo(CreateTodoRequest request)
    {
        try
        {
            _logger.LogInformation("Creating new todo: {TodoTitle}", request.Title);
            var todo = await _mediator.Send(new CreateTodoCommand(request));
            return CreatedAtAction(nameof(GetTodo), new { id = todo.Id }, todo);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating todo");
            return StatusCode(500, "An error occurred while creating the todo");
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<TodoDto>> UpdateTodo(int id, UpdateTodoRequest request)
    {
        try
        {
            _logger.LogInformation("Updating todo with id {TodoId}", id);
            var todo = await _mediator.Send(new UpdateTodoCommand(id, request));
            return Ok(todo);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Todo with id {TodoId} not found for update", id);
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating todo with id {TodoId}", id);
            return StatusCode(500, "An error occurred while updating the todo");
        }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteTodo(int id)
    {
        try
        {
            _logger.LogInformation("Deleting todo with id {TodoId}", id);
            var result = await _mediator.Send(new DeleteTodoCommand(id));
            
            if (!result)
            {
                _logger.LogWarning("Todo with id {TodoId} not found for deletion", id);
                return NotFound();
            }

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting todo with id {TodoId}", id);
            return StatusCode(500, "An error occurred while deleting the todo");
        }
    }
}