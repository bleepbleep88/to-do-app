using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;

namespace TodoApp.Infrastructure.Middleware;

public class AuthMiddleware
{
    private readonly RequestDelegate _next;
    private readonly string _secretKey;

    public AuthMiddleware(RequestDelegate next)
    {
        _next = next;
        _secretKey = "super-secret-key-for-demo-purposes-must-be-at-least-32-chars";
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var path = context.Request.Path.Value?.ToLower();
        
        // Skip authentication for public endpoints
        if (IsPublicEndpoint(path))
        {
            await _next(context);
            return;
        }

        var token = ExtractTokenFromHeader(context.Request.Headers.Authorization);
        
        if (string.IsNullOrEmpty(token))
        {
            context.Response.StatusCode = 401;
            await context.Response.WriteAsync("Unauthorized: No token provided");
            return;
        }

        if (!ValidateToken(token, out var claims))
        {
            context.Response.StatusCode = 401;
            await context.Response.WriteAsync("Unauthorized: Invalid token");
            return;
        }

        // Set the user claims in the context
        context.User = new ClaimsPrincipal(new ClaimsIdentity(claims, "Bearer"));
        
        await _next(context);
    }

    private static bool IsPublicEndpoint(string? path)
    {
        if (string.IsNullOrEmpty(path)) return false;
        
        return path.Contains("/auth/login") || 
               path.Contains("/auth/register") || 
               path.Contains("/swagger") || 
               path.Contains("/health") ||
               path == "/";
    }

    private static string? ExtractTokenFromHeader(Microsoft.Extensions.Primitives.StringValues authorizationHeader)
    {
        if (authorizationHeader.Count == 0) return null;
        
        var header = authorizationHeader.FirstOrDefault();
        if (string.IsNullOrEmpty(header)) return null;
        
        return header.StartsWith("Bearer ") ? header.Substring(7) : null;
    }

    private bool ValidateToken(string token, out List<Claim> claims)
    {
        claims = new List<Claim>();
        
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_secretKey);
            
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false,
                ClockSkew = TimeSpan.Zero
            };

            var principal = tokenHandler.ValidateToken(token, validationParameters, out _);
            claims.AddRange(principal.Claims);
            return true;
        }
        catch
        {
            return false;
        }
    }
}