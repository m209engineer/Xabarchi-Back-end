namespace Xabarchi.Application.Abstractions;

public interface IJwtService
{
    string GenerateToken(Guid userId, string username);
}