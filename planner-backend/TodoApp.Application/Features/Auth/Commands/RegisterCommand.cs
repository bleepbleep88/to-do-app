using MediatR;
using System.Text.RegularExpressions;
using TodoApp.Application.Common.DTOs;
using TodoApp.Application.Common.Interfaces;
using TodoApp.Domain.Entities;

namespace TodoApp.Application.Features.Auth.Commands;

public record RegisterCommand(RegisterRequest Request) : IRequest<AuthResponse>;

public partial class RegisterCommandHandler : IRequestHandler<RegisterCommand, AuthResponse>
{
    private readonly IUserRepository _userRepository;
    private readonly IAuthService _authService;

    [GeneratedRegex(@"[A-Z]", RegexOptions.None, matchTimeoutMilliseconds: 1000)]
    private static partial Regex UppercaseRegex();

    [GeneratedRegex(@"\d", RegexOptions.None, matchTimeoutMilliseconds: 1000)]
    private static partial Regex DigitRegex();

    public RegisterCommandHandler(IUserRepository userRepository, IAuthService authService)
    {
        _userRepository = userRepository;
        _authService = authService;
    }

    public async Task<AuthResponse> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        // Validate password requirements
        ValidatePassword(request.Request.Password);
        
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

    private static void ValidatePassword(string password)
    {
        if (string.IsNullOrEmpty(password))
            throw new ArgumentException("Password is required");

        if (password.Length < 8)
            throw new ArgumentException("Password must be at least 8 characters long");

        if (!UppercaseRegex().IsMatch(password))
            throw new ArgumentException("Password must contain at least one uppercase letter");

        if (!DigitRegex().IsMatch(password))
            throw new ArgumentException("Password must contain at least one number");
    }
}