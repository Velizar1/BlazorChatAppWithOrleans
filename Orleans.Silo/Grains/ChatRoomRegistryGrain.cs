using Orleans.Interfaces.IGrains;
using Orleans.Interfaces.Models;
using Orleans.Silo.States;

namespace Orleans.Silo.Grains
{
    public class ChatRoomRegistryGrain : Grain<ChatRoomRegistryState>, IChatRoomRegistryGrain
    {

        public async Task<bool> AddRoom(string username, string roomName)
        {
            if (!State.ChatRooms.Values.Contains(roomName)) 
            {
                var key = new Guid();

                var chatRoomGrain = this.GrainFactory.GetGrain<IChatRoomGrain>(key);
                
                State.ChatRooms.Add(key, roomName);
                await WriteStateAsync();

                await chatRoomGrain.InitializeRoom(username, roomName);

                return true;
            }

            return false;
        }

        public async Task<List<string>> GetRooms()
        {
            return State.ChatRooms.Values.ToList();
        }
    }
}
