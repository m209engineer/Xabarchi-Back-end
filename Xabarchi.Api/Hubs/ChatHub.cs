using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;
using Xabarchi.Application.Abstractions;

namespace Xabarchi.Api.Hubs;

[Authorize]
public class ChatHub : Hub
{
    private readonly IUserService _userService;

    public ChatHub(IUserService userService)
    {
        _userService = userService;
    }

    public override async Task OnConnectedAsync()
    {
        var userId = GetUserId();
        if (userId != null)
        {
            await _userService.UpdateOnlineStatus(userId.Value, true);
            await Clients.All.SendAsync("UserStatusChanged", new
            {
                userId = userId.ToString(),
                isOnline = true
            });
        }
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var userId = GetUserId();
        if (userId != null)
        {
            await _userService.UpdateOnlineStatus(userId.Value, false);
            await _userService.UpdateLastSeen(userId.Value);
            await Clients.All.SendAsync("UserStatusChanged", new
            {
                userId = userId.ToString(),
                isOnline = false,
                lastSeen = DateTime.UtcNow
            });
        }
        await base.OnDisconnectedAsync(exception);
    }

    public async Task JoinChat(string chatId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, chatId);
    }

    public async Task LeaveChat(string chatId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, chatId);
    }

    public async Task Typing(string chatId)
    {
        var userId = Context.User!.FindFirstValue(ClaimTypes.NameIdentifier);
        await Clients.OthersInGroup(chatId).SendAsync("UserTyping", userId);
    }

    private Guid? GetUserId()
    {
        var value = Context.User?.FindFirstValue(ClaimTypes.NameIdentifier);
        return Guid.TryParse(value, out var id) ? id : null;
    }
}