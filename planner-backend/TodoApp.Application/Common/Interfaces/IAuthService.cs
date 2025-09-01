using TodoApp.Domain.Entities;

namespace TodoApp.Application.Common.Interfaces;

public interface IAuthService
{
    string GenerateToken(User user);
    string HashPassword(string password);
    bool VerifyPassword(string password, string hashedPassword);
    int GetCurrentUserId();
}