using Orleans.Interfaces.IGrains;
using Orleans.Interfaces.Models;
using Orleans.Silo.States;

namespace Orleans.Silo.Grains
{
    public class ChatRoomRegistryGrain : Grain<ChatRoomRegistryState>, IChatRoomRegistryGrain
    {

        public async Task<Guid?> AddRoom(string username, string roomName)
        {
            if (!State.ChatRooms.Values.Contains(roomName)) 
            {
                var key = new Guid();

                var chatRoomGrain = this.GrainFactory.GetGrain<IChatRoomGrain>(key);
                
                State.ChatRooms.Add(key, roomName);
                await WriteStateAsync();

                await chatRoomGrain.InitializeRoom(username, roomName);

                return key;
            }

            return null;
        }

        public ValueTask<Guid> GetRoomIdByName(string roomName)
        {
            return ValueTask.FromResult(State.ChatRooms
                    .FirstOrDefault(pair => pair.Value == roomName)
                    .Key);
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

                var chatRoomGrain = this.GrainFactory.GetGrain<IChatRoomGrain>(roomKey);

                var isIserAdded = await chatRoomGrain.AddUserToChatRoom(username);

                return isIserAdded;
            }

            return false;
        }
    }
}

