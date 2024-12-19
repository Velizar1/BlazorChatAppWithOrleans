using Microsoft.AspNetCore.SignalR;
using Orleans.Interfaces.IGrains;
using Orleans.Interfaces.Models;

namespace Blazor.Client.Hubs
{
    public class ChatHub : Hub
    {
        private readonly IClusterClient _client;

        public ChatHub(IClusterClient client)
        {
            _client = client;
        }

        public async Task CreateRoom(string roomName, string username)
        {
            await Clients.All.SendAsync("ReceiveRoom", roomName);
        }

        public async Task AddUserToRoom(string roomname, string userName)
        {
            var chatMessage = new ChatMessage
            {
                CreatedAt = DateTime.Now,
                Message = $"{userName} has joined",
                Sender = roomname,
            };

            await Groups.AddToGroupAsync(Context.ConnectionId, roomname);
            await Clients.Group(roomname).SendAsync("ReceiveMessage", chatMessage);
        }

        public async Task RemoveUserFromRoom(string roomName, string userName)
        {
            var chatMessage = new ChatMessage
            {
                CreatedAt = DateTime.Now,
                Message = $"{userName} has left",
                Sender = roomName,
            };

            await Groups.RemoveFromGroupAsync(Context.ConnectionId, roomName);
            await Clients.Group(roomName).SendAsync("ReceiveMessage", chatMessage);
        }

        public async Task SendMessage(Guid roomId, string roomname, string userName, string message)
        {
            var chatRoom = _client.GetGrain<IChatRoomGrain>(roomId);

            var chatMessage = new ChatMessage
            {
                CreatedAt = DateTime.Now,
                Message = message,
                Sender = userName,
            };

            var isSent = await chatRoom.SendMessage(chatMessage);

            await Clients.Group(roomname).SendAsync("ReceiveMessage", chatMessage);
        }
    }
}
