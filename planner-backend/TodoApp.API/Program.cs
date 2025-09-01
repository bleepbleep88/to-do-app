using Microsoft.EntityFrameworkCore;
using MediatR;
using Serilog;
using TodoApp.Infrastructure.Data;
using TodoApp.Infrastructure.Repositories;
using TodoApp.Infrastructure.Services;
using TodoApp.Infrastructure.Middleware;
using TodoApp.Application.Common.Interfaces;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("logs/app-.log", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add HttpContextAccessor
builder.Services.AddHttpContextAccessor();

// Add Entity Framework with In-Memory Database
builder.Services.AddDbContext<TodoDbContext>(options =>
    options.UseInMemoryDatabase("TodoAppDb"));

// Add MediatR
var applicationAssembly = Assembly.Load("TodoApp.Application");
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(applicationAssembly));

// Add AutoMapper
builder.Services.AddAutoMapper(typeof(Program));

// Add Memory Caching
builder.Services.AddMemoryCache();

// Register repositories and services
builder.Services.AddScoped<ITodoRepository, TodoRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ISubtaskRepository, SubtaskRepository>();
builder.Services.AddScoped<IAuthService, AuthService>();

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Use CORS
app.UseCors("AllowAll");

app.UseHttpsRedirection();

// Use custom authentication middleware
app.UseMiddleware<AuthMiddleware>();

app.UseAuthorization();

app.MapControllers();

// Ensure database is created and seeded
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<TodoDbContext>();
    context.Database.EnsureCreated();
}

app.Run();
