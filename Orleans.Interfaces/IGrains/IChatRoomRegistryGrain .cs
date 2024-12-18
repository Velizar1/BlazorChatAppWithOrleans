namespace Orleans.Interfaces.IGrains
{
    public interface IChatRoomRegistryGrain : IGrainWithIntegerKey
    {
        Task<List<string>> GetRooms();

        Task<bool> AddRoom(string user, string roomName);
    }
}
