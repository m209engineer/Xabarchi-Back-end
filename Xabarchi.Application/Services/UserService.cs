using Microsoft.EntityFrameworkCore;
using Xabarchi.Application.Abstractions;
using Xabarchi.Domain.DTOs;
using Xabarchi.Domain.Model;
using Xabarchi.Domain.ResponseDTOs;
using Xabarchi.Domain.SimpleDTOs;
using Xabarchi.Infrastructure.Persistance;

namespace Xabarchi.Application.Services;

public class UserService : IUserService
{
    private readonly ApplicationDbContext _context;
    private readonly ICloudinaryService _cloudinaryService;
    private readonly IChatHubService _hubService;

    public UserService(ApplicationDbContext context, ICloudinaryService cloudinaryService, IChatHubService hubService)
    {
        _context = context;
        _cloudinaryService = cloudinaryService;
        _hubService = hubService;  
    }

    public async Task<string> UpdateOnlineStatus(Guid userId, bool isOnline)
    {
        var user = await _context.Users.FindAsync(userId);

        if (user == null)
            throw new Exception("User not found");

        user.IsOnline = isOnline;
        user.UpdatedAt = DateTime.UtcNow;

        if (!isOnline)
            user.LastSeen = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        await _hubService.UserStatusChanged(new
        {
            UserId = userId,
            IsOnline = isOnline,
            LastSeen = user.LastSeen
        });

        return "Online status updated";
    }

    public async Task<UserResponseDto> GetMyProfile(Guid userId)
    {
        var user = await _context.Users
            .Include(u => u.Chatmembers)
                .ThenInclude(cm => cm.Chat)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null)
            throw new Exception("User not found");

        return MapToResponseDto(user);
    }

    public async Task<UserResponseDto> GetUserById(Guid userId)
    {
        var user = await _context.Users
            .Include(u => u.Chatmembers)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null)
            throw new Exception("User not found");

        return MapToResponseDto(user);
    }

    public async Task<string> UpdateProfile(Guid userId, UserDto dto)
    {
        var user = await _context.Users.FindAsync(userId);

        if (user == null)
            throw new Exception("User not found");

        user.Firstname = dto.Firstname;
        user.Lastname = dto.Lastname;
        user.Bio = dto.Bio;
        user.Birthday = dto.Birthday;

        if (dto.AvatarFile != null)
        {
            var uploadResult = await _cloudinaryService.UploadAvatar(dto.AvatarFile);
            user.AvatarUrl = uploadResult.Url;
        }

        user.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return "Profile updated successfully";
    }

    public async Task<string> DeleteAvatar(Guid userId)
    {
        var user = await _context.Users.FindAsync(userId);

        if (user == null)
            throw new Exception("User not found");

        user.AvatarUrl = null;
        user.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return "Avatar deleted successfully";
    }

    public async Task<UserResponseDto?> GetUserByUsername(string username)
    {
        var user = await _context.Users
            .Include(u => u.Chatmembers)
            .FirstOrDefaultAsync(u => u.Username == username);

        return user == null ? null : MapToResponseDto(user);
    }

    public async Task<bool> CheckUsernameAvailable(string username)
    {
        return !await _context.Users.AnyAsync(u => u.Username == username);
    }

    public async Task<bool> ChangeUsername(Guid userId, string newUsername)
    {
        var usernameExists = await _context.Users.AnyAsync(u => u.Username == newUsername);
        if (usernameExists) return false;

        var user = await _context.Users.FindAsync(userId);
        if (user == null) return false;

        user.Username = newUsername;
        user.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<List<UserSimpleDto>> SearchUsers(string query)
    {
        var users = await _context.Users
            .Where(u => u.Username.Contains(query) ||
                       (u.Firstname != null && u.Firstname.ToLower().Contains(query.ToLower())) ||
                       (u.Lastname != null && u.Lastname.ToLower().Contains(query.ToLower())))
            .Take(20)
            .ToListAsync();

        return users.Select(u => new UserSimpleDto
        {
            Id = u.Id,
            Username = u.Username, 
            Firstname = u.Firstname,
            Lastname = u.Lastname,
            AvatarUrl = u.AvatarUrl,
            IsOnline = u.IsOnline,
            LastSeen = u.LastSeen
        }).ToList();
    }

    public async Task<string> UpdateLastSeen(Guid userId)
    {
        var user = await _context.Users.FindAsync(userId);

        if (user == null)
            throw new Exception("User not found");

        user.LastSeen = DateTime.UtcNow;
        user.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return "Last seen updated";
    }

    public async Task<bool> GetOnlineStatus(Guid userId)
    {
        var user = await _context.Users.FindAsync(userId);
        return user?.IsOnline ?? false;
    }

    public async Task<bool> UpdateEmail(Guid userId, string email)
    {
        var emailExists = await _context.Users.AnyAsync(u => u.Email == email);
        if (emailExists) return false;

        var user = await _context.Users.FindAsync(userId);
        if (user == null) return false;

        user.Email = email;
        user.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> UpdatePhone(Guid userId, string phone)
    {
        var phoneExists = await _context.Users.AnyAsync(u => u.Phone == phone);
        if (phoneExists) return false;

        var user = await _context.Users.FindAsync(userId);
        if (user == null) return false;

        user.Phone = phone;
        user.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return true;
    }
    public async Task<string> GetUserAvatar(Guid userId)
    {
        var avatarUrl = await _context.Users
            .Where(u => u.Id == userId)
            .Select(u => u.AvatarUrl)
            .FirstOrDefaultAsync();

        return avatarUrl ?? string.Empty;
    }

    private UserResponseDto MapToResponseDto(User user)
    {
        return new UserResponseDto
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email,
            Phone = user.Phone,
            Firstname = user.Firstname,
            Lastname = user.Lastname,
            Bio = user.Bio,
            AvatarUrl = user.AvatarUrl,
            Birthday = user.Birthday,
            IsOnline = user.IsOnline,
            LastSeen = user.LastSeen,
            CreatedAt = user.CreatedAt,
            Chatmembers = user.Chatmembers.Select(cm => new ChatmemberSimpleDto
            {
                Id = cm.Id,
                JoinedAt = cm.JoinedAt,
                User = new UserSimpleDto
                {
                    Id = user.Id,
                    Firstname = user.Firstname,
                    Lastname = user.Lastname,
                    Username = user.Username,
                    AvatarUrl = user.AvatarUrl,
                    IsOnline = user.IsOnline,
                    LastSeen = user.LastSeen
                }
            }).ToList()
        };
    }
}