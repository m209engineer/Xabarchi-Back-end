using Xabarchi.Domain.DTOs;
using Xabarchi.Domain.ResponseDTOs;
using Xabarchi.Domain.SimpleDTOs;

namespace Xabarchi.Application.Abstractions;

public interface IUserService
{
    // Profile Management
    Task<UserResponseDto> GetMyProfile(Guid userId);
    Task<UserResponseDto> GetUserById(Guid userId);
    Task<string> UpdateProfile(Guid userId, UserDto dto);       
    Task<string> DeleteAvatar(Guid userId);                       
    
    // Username
    Task<UserResponseDto?> GetUserByUsername(string username);  
    Task<string> GetUserAvatar(Guid userId);  
    Task<bool> CheckUsernameAvailable(string username);         
    Task<bool> ChangeUsername(Guid userId, string newUsername);   
    
    // Search
    Task<List<UserSimpleDto>> SearchUsers(string query);    
    
    // Online Status
    Task<string> UpdateOnlineStatus(Guid userId, bool isOnline); 
    Task<string> UpdateLastSeen(Guid userId);                     
    Task<bool> GetOnlineStatus(Guid userId);                   
    
    // Email & Phone
    Task<bool> UpdateEmail(Guid userId, string email);         
    Task<bool> UpdatePhone(Guid userId, string phone);            
}