using Xabarchi.Domain.AuthDto;

namespace Xabarchi.Application.Abstractions;

public interface IAuthService
{
    Task<TokenResponse> Register(RegisterDto model);
    Task<TokenResponse> Login(LoginDto model);
    Task<bool> ChangePassword(Guid userId, string oldPassword, string newPassword);
    Task<bool> Logout(Guid userId);
}