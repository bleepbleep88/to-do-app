using MediatR;
using TodoApp.Application.Common.DTOs;
using TodoApp.Application.Common.Interfaces;

namespace TodoApp.Application.Features.Auth.Commands;

public record LoginCommand(LoginRequest Request) : IRequest<AuthResponse>;

public class LoginCommandHandler : IRequestHandler<LoginCommand, AuthResponse>
{
    private readonly IUserRepository _userRepository;
    private readonly IAuthService _authService;

    public LoginCommandHandler(IUserRepository userRepository, IAuthService authService)
    {
        _userRepository = userRepository;
        _authService = authService;
    }

    public async Task<AuthResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByUsernameAsync(request.Request.Username);
        
        if (user == null || !_authService.VerifyPassword(request.Request.Password, user.PasswordHash))
            throw new UnauthorizedAccessException("Invalid credentials");

        if (!user.IsActive)
            throw new UnauthorizedAccessException("Account is disabled");

        user.LastLoginAt = DateTime.UtcNow;
        await _userRepository.UpdateAsync(user);

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