namespace Xabarchi.Application.Abstractions;

public interface IChatHubService
{
    Task SendMessage(string chatId, object message);
    Task MessageEdited(string chatId, object message);
    Task MessageDeleted(string chatId, object data);
    Task MessageRead(string chatId, object data);
    Task ReactionsUpdated(string chatId, object data);
    Task UserStatusChanged(object data);
    Task NewChat(string userId, object chat);
    Task ChatDeleted(string chatId, object data);
}