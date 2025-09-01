using MediatR;
using TodoApp.Application.Common.DTOs;
using TodoApp.Application.Common.Interfaces;
using TodoApp.Domain.Entities;

namespace TodoApp.Application.Features.Auth.Commands;

public record RegisterCommand(RegisterRequest Request) : IRequest<AuthResponse>;

public class RegisterCommandHandler : IRequestHandler<RegisterCommand, AuthResponse>
{
    private readonly IUserRepository _userRepository;
    private readonly IAuthService _authService;

    public RegisterCommandHandler(IUserRepository userRepository, IAuthService authService)
    {
        _userRepository = userRepository;
        _authService = authService;
    }

    public async Task<AuthResponse> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        if (await _userRepository.UsernameExistsAsync(request.Request.Username))
            throw new ArgumentException("Username already exists");

        if (await _userRepository.EmailExistsAsync(request.Request.Email))
            throw new ArgumentException("Email already exists");

        var now = DateTime.UtcNow;
        var user = new User
        {
            Username = request.Request.Username,
            Email = request.Request.Email,
            PasswordHash = _authService.HashPassword(request.Request.Password),
            FirstName = request.Request.FirstName,
            LastName = request.Request.LastName,
            IsActive = true,
            CreatedAt = now,
            UpdatedAt = now,
            LastLoginAt = now
        };

        await _userRepository.AddAsync(user);

        var token = _authService.GenerateToken(user);

        return new AuthResponse
        {
            Token = token,
            User = new UserDto
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                IsActive = user.IsActive,
                CreatedAt = user.CreatedAt,
                LastLoginAt = user.LastLoginAt
            },
            ExpiresAt = DateTime.UtcNow.AddHours(24)
        };
    }
}