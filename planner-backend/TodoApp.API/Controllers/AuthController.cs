using MediatR;
using Microsoft.AspNetCore.Mvc;
using TodoApp.Application.Common.DTOs;
using TodoApp.Application.Features.Auth.Commands;

namespace PlannerAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IMediator mediator, ILogger<AuthController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpPost("register")]
    public async Task<ActionResult<AuthResponse>> Register(RegisterRequest request)
    {
        try
        {
            _logger.LogInformation("User registration attempt for username: {Username}", request.Username);
            var response = await _mediator.Send(new RegisterCommand(request));
            _logger.LogInformation("User successfully registered: {Username}", request.Username);
            return Ok(response);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning("Registration failed for username {Username}: {Error}", request.Username, ex.Message);
            return BadRequest(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during user registration for username: {Username}", request.Username);
            return StatusCode(500, new { error = "An error occurred during registration" });
        }
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login(LoginRequest request)
    {
        try
        {
            _logger.LogInformation("Login attempt for username: {Username}", request.Username);
            var response = await _mediator.Send(new LoginCommand(request));
            _logger.LogInformation("User successfully logged in: {Username}", request.Username);
            return Ok(response);
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning("Login failed for username {Username}: {Error}", request.Username, ex.Message);
            return Unauthorized(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during login for username: {Username}", request.Username);
            return StatusCode(500, new { error = "An error occurred during login" });
        }
    }
}