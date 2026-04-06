using Xabarchi.Application.Abstractions;
using Xabarchi.Domain.AuthDto;
using Xabarchi.Domain.Model;
using Xabarchi.Infrastructure.Persistance;
using Microsoft.EntityFrameworkCore;
using BCrypt.Net;

namespace Xabarchi.Application.Services;

public class AuthService : IAuthService
{
    private readonly ApplicationDbContext _context;
    private readonly IJwtService _jwtService;

    public AuthService(ApplicationDbContext context, IJwtService jwtService)
    {
        _context = context;
        _jwtService = jwtService;
    }

    public async Task<TokenResponse> Register(RegisterDto model)
    {
        var existingUser = await _context.Users
            .FirstOrDefaultAsync(u => u.Username == model.Username);
        
        if (existingUser != null)
        {
            return new TokenResponse
            {
                Success = false,
                Message = "Username already exists"
            };
        }


        if (!string.IsNullOrEmpty(model.Email))
        {
            var emailExists = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == model.Email);
            
            if (emailExists != null)
            {
                return new TokenResponse
                {
                    Success = false,
                    Message = "Email already exists"
                };
            }
        }

        if (!string.IsNullOrEmpty(model.Phone))
        {
            var phoneExists = await _context.Users
                .FirstOrDefaultAsync(u => u.Phone == model.Phone);
            
            if (phoneExists != null)
            {
                return new TokenResponse
                {
                    Success = false,
                    Message = "Phone already exists"
                };
            }
        }

        var passwordHash = BCrypt.Net.BCrypt.HashPassword(model.Password);

        var user = new User
        {
            Id = Guid.NewGuid(),
            Username = model.Username,
            PasswordHash = passwordHash,
            Email = model.Email,
            Phone = model.Phone,
            IsOnline = true,
            CreatedAt = DateTime.UtcNow
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        var token = _jwtService.GenerateToken(user.Id, user.Username);

        return new TokenResponse
        {
            Success = true,
            Message = "User registered successfully",
            Token = token,
            UserId = user.Id,
            Username = user.Username
        };
    }

    public async Task<TokenResponse> Login(LoginDto model)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Username == model.Username);

        if (user == null)
        {
            return new TokenResponse
            {
                Success = false,
                Message = "Invalid username or password"
            };
        }

        var isValidPassword = BCrypt.Net.BCrypt.Verify(model.Password, user.PasswordHash);

        if (!isValidPassword)
        {
            return new TokenResponse
            {
                Success = false,
                Message = "Invalid username or password"
            };
        }

        user.IsOnline = true;
        user.LastSeen = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        var token = _jwtService.GenerateToken(user.Id, user.Username);

        return new TokenResponse
        {
            Success = true,
            Message = "Login successful",
            Token = token,
            UserId = user.Id,
            Username = user.Username
        };
    }

    public async Task<bool> ChangePassword(Guid userId, string oldPassword, string newPassword)
    {
        var user = await _context.Users.FindAsync(userId);

        if (user == null)
        {
            return false;
        }

        var isValidPassword = BCrypt.Net.BCrypt.Verify(oldPassword, user.PasswordHash);

        if (!isValidPassword)
        {
            return false;
        }

        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
        user.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return true;
    }
    
    public async Task<bool> Logout(Guid userId)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null) return false;

        user.IsOnline = false;
        user.LastSeen = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return true;
    }
}