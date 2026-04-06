<div align="center">

# Xabarchi — Backend

Real-time messaging API built with ASP.NET Core 8 and SignalR

![ASP.NET Core](https://img.shields.io/badge/ASP.NET%20Core-8.0-818cf8?style=flat-square&labelColor=1e2535)
![SignalR](https://img.shields.io/badge/SignalR-real--time-5eead4?style=flat-square&labelColor=1e2535)
![PostgreSQL](https://img.shields.io/badge/PostgreSQL-EF%20Core-93c5fd?style=flat-square&labelColor=1e2535)
![JWT](https://img.shields.io/badge/Auth-JWT-fcd34d?style=flat-square&labelColor=1e2535)

</div>

---

## Stack

| | Technology |
|---|---|
| Framework | ASP.NET Core 8 |
| Real-time | SignalR |
| Database | PostgreSQL + Entity Framework Core |
| Auth | JWT Bearer Tokens |
| File Storage | Supabase |


---

## Project Structure

Clean Architecture — four layers, each with a single responsibility:

```
Xabarchi.Api            →  controllers, SignalR hubs, middleware
Xabarchi.Application    →  service abstractions, use cases, DTOs
Xabarchi.Domain         →  entities, models, domain DTOs
Xabarchi.Infrastructure →  EF Core, repositories, Supabase, Cloudinary, JWT
```

---

## What it does

**Auth**
- Register and login with JWT tokens
- Change password, logout

**Messaging**
- Send, edit, delete and reply to messages
- Read receipts and unread message count per chat
- Paginated message history

**Media**
- Attach images, videos, audio and voice messages to messages
- Each media item can be deleted independently

**Reactions**
- Add and remove emoji reactions on messages
- Get all reactions for a message

**Chats**
- Create and delete chats
- Get or create a chat between two users automatically

**Users**
- Profile management — update info, avatar, username, email, phone
- User search by query
- Online status and last seen tracking

---

## SignalR Hub

Connect to `/hubs/chat` — all events are pushed server → client in real time.

| Event | Description |
|---|---|
| `SendMessage` | New message delivered to all chat members |
| `MessageEdited` | Edited message broadcast to the chat |
| `MessageDeleted` | Deletion notification sent to chat members |
| `MessageRead` | Read receipt sent back to the sender |
| `ReactionsUpdated` | Reaction added or removed on a message |
| `UserStatusChanged` | User came online or went offline |
| `NewChat` | New chat pushed to the relevant user |
| `ChatDeleted` | Chat deletion broadcast to all members |

---


---

<div align="center">
  <sub>Built by <a href="https://github.com/m209engineer">m209engineer</a></sub>
</div>
