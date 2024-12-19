namespace Orleans.Interfaces.IGrains
{
    public interface IChatRoomRegistryGrain : IGrainWithIntegerKey
    {
        ValueTask<List<string>> GetRooms();

        ValueTask<string> GetRoomIdByName(string roomName);

        Task<Guid?> AddRoom(string user, string roomName);

        Task<bool> JoinChat(string username, string roomName);
    }
}
