using Microsoft.Extensions.DependencyInjection;

using Xabarchi.Application.Abstractions;
using Xabarchi.Application.Services;
using Xabarchi.Infrastructure.Services;

namespace Xabarchi.Application;

public static class ApllicationDependencyInjecton
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // Auth
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IJwtService, JwtService>();
        
        // User
        services.AddScoped<IUserService, UserService>();
        
        // Chat
        services.AddScoped<IChatService, ChatService>();
        
        // Message
        services.AddScoped<IMessageService, MessageService>();

        services.AddScoped<IReactionService, ReactionService>();
        
        
        // Media
        services.AddScoped<IMediaService, MediaService>();
        
        // Cloudinary 
        services.AddScoped<ICloudinaryService, CloudinaryService>();

        return services;
    }
}