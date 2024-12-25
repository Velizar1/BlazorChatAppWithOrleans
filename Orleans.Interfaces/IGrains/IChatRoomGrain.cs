using Orleans.Interfaces.Models;

namespace Orleans.Interfaces.IGrains
{
    public interface IChatRoomGrain : IGrainWithGuidKey
    {
        Task<bool> InitializeRoom(string username, string roomName);

        Task<bool> AddUserToChatRoom(string username);

        Task<bool> RemoveUserFromChatRoom(string username);

        Task<bool> SendMessage(ChatMessage message);

        Task<HashSet<string>> GetUsers();

        Task<List<ChatMessage>> GetMessages(int count = 30);

        Task<List<ChatMessage>> LoadOlderMessages(int skip, int take);
        Task<string> GetRoomName();
    }
}
