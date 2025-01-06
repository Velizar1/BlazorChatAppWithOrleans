using Microsoft.AspNetCore.SignalR;
using Orleans.Interfaces.IGrains;
using Orleans.Interfaces.Models;
namespace Orleans.Silo.Hubs
{
    public class ChatHub : Hub
    {
        private readonly IGrainFactory _grainFactory;

        public ChatHub(IGrainFactory grainFactory)
        {
            _grainFactory = grainFactory;
        }

        public async Task<bool> RegisterUser(string username)
        {
            var userRegistry = _grainFactory.GetGrain<IUserRegistryGrain>(0);
            var isSucceeded = await userRegistry.RegisterUser(username);

            return isSucceeded;
        }

        public async Task<List<string>> GetRooms()
        {

            var chatRoomRegistry = _grainFactory.GetGrain<IChatRoomRegistryGrain>(1);
            var chatRoomsList = await chatRoomRegistry.GetRooms();


            return chatRoomsList;
        }

        public async Task<Guid?> CreateRoom(string roomName, string username)
        {
            var chatRoomRegistry = _grainFactory.GetGrain<IChatRoomRegistryGrain>(1);
            var roomId = await chatRoomRegistry.AddRoom(username, roomName);

            if (roomId != null)
            {
                await Clients.All.SendAsync("ReceiveRoom", roomName);
            }

            return roomId;
        }

        public async Task<string> JoinRoom(string roomName, string username)
        {
            var chatRoomRegistry = _grainFactory.GetGrain<IChatRoomRegistryGrain>(1);
            var isSucceeded = await chatRoomRegistry.JoinChat(username, roomName);
            
            if (!isSucceeded)
            {
                return null;
            }

            var chatMessage = new ChatMessage
            {
                CreatedAt = DateTime.Now,
                Message = $"{username} has joined",
            };

            var roomId = await chatRoomRegistry.GetRoomIdByName(roomName);

            await Groups.AddToGroupAsync(Context.ConnectionId, roomName);
            await Clients.Group(roomName).SendAsync("ReceiveMessage", chatMessage);

            return roomId;
        }

        public async Task<bool> RemoveUserFromRoom(string roomId, string username)
        {
            var chatRoom = _grainFactory.GetGrain<IChatRoomGrain>(Guid.Parse(roomId));
            // Check if user is successfully removed from the chat grain state
            var isRemoved = await chatRoom.RemoveUserFromChatRoom(username);
            
            if (!isRemoved)
            {
                return false;
            }

            var chatMessage = new ChatMessage
            {
                CreatedAt = DateTime.Now,
                Message = $"{username} has left",
            };

            var roomName = await chatRoom.GetRoomName();

            await Groups.RemoveFromGroupAsync(Context.ConnectionId, roomName);
            await Clients.Group(roomName).SendAsync("ReceiveMessage", chatMessage);

            return true;
        }

        public async Task SendMessage(Guid roomId, string userName, string message)
        {
            var chatRoom = _grainFactory.GetGrain<IChatRoomGrain>(roomId);

            var chatMessage = new ChatMessage
            {
                CreatedAt = DateTime.Now,
                Message = message,
                Sender = userName,
            };

            var isSent = await chatRoom.SendMessage(chatMessage);
            var roomname = await chatRoom.GetRoomName();

            await Clients.Group(roomname).SendAsync("ReceiveMessage", chatMessage);
        }
        
        public async Task<HashSet<string>> GetRoomUsers(string roomId)
        {

            var chatRoom = _grainFactory.GetGrain<IChatRoomGrain>(Guid.Parse(roomId));
            var users = await chatRoom.GetUsers();


            return users ?? new HashSet<string>();
        }

        public async Task<List<ChatMessage>> GetRoomMessages(string roomId)
        {
            var chatRoom = _grainFactory.GetGrain<IChatRoomGrain>(Guid.Parse(roomId));

            var chatRoomMessages = await chatRoom.GetMessages();

            return chatRoomMessages;
        }

        public async Task<List<ChatMessage>> LoadOlderMessages(string roomId, int skip, int take)
        {
            var chatRoom = _grainFactory.GetGrain<IChatRoomGrain>(Guid.Parse(roomId));
           
            var chatRoomMessages = await chatRoom.LoadOlderMessages(skip, take);

            return chatRoomMessages;
        }

        public override async Task OnConnectedAsync()
        {
            var roomname = Context.GetHttpContext()?.Request.Query["roomname"];

            if (!string.IsNullOrEmpty(roomname))
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, roomname);
            }

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var roomname = Context.GetHttpContext()?.Request.Query["roomname"];

            if (!string.IsNullOrEmpty(roomname))
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, roomname);
            }

            await base.OnDisconnectedAsync(exception);
        }
    }
}
