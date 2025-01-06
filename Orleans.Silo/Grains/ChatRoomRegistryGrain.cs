using Orleans.Interfaces.IGrains;
using Orleans.Interfaces.Models;
using Orleans.Silo.States;

namespace Orleans.Silo.Grains
{
    public class ChatRoomRegistryGrain : Grain<ChatRoomRegistryState>, IChatRoomRegistryGrain
    {

        public async Task<Guid?> AddRoom(string username, string roomName)
        {
            if (!string.IsNullOrWhiteSpace(username) || !State.ChatRooms.Values.Contains(roomName) ) 
            {
                var key = Guid.NewGuid();

                var chatRoomGrain = this.GrainFactory.GetGrain<IChatRoomGrain>(key);
                
                State.ChatRooms.Add(key.ToString(), roomName);
                await WriteStateAsync();

                await chatRoomGrain.InitializeRoom(username, roomName);

                return key;
            }

            return null;
        }

        public ValueTask<string> GetRoomIdByName(string roomName)
        {
            return ValueTask.FromResult(State.ChatRooms
                    .FirstOrDefault(pair => pair.Value == roomName)
                    .Key.ToString());
        }

        public ValueTask<List<string>> GetRooms()
        {
            return ValueTask.FromResult(State.ChatRooms.Values.ToList());
        }

        public async Task<bool> JoinChat(string username, string roomName)
        {
            if (State.ChatRooms.Values.Contains(roomName))
            {
                var roomKey = State.ChatRooms
                    .FirstOrDefault(pair => pair.Value == roomName)
                    .Key;

                var chatRoomGrain = this.GrainFactory.GetGrain<IChatRoomGrain>(Guid.Parse(roomKey));
                var users = await chatRoomGrain.GetUsers();
                
                if (!users.Contains(username))
                {
                    var isUserAdded = await chatRoomGrain.AddUserToChatRoom(username);
                    return isUserAdded;
                }

                return true;
            }

            return false;
        }
    }
}

