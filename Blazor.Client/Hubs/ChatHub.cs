using Blazor.Client.Pages;
using Microsoft.AspNetCore.SignalR;
using Orleans.Interfaces.IGrains;
using Orleans.Interfaces.Models;
using System.Drawing;

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
            await Groups.AddToGroupAsync(Context.ConnectionId, roomname);
            await Clients.Group(roomname).SendAsync("ReceiveMessage", ($"{userName} has joined", Color.Green.Name));
        }

        public async Task RemoveUserFromRoom(string roomName, string userName)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, roomName);
            await Clients.Group(roomName).SendAsync("ReceiveMessage", ($"{userName} has left", Color.Red.Name));
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

            await Clients.Group(roomname).SendAsync("ReceiveMessage", userName, message);
        }
    }
}
