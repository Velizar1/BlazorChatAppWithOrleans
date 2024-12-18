using Orleans.Interfaces.IGrains;
using Orleans.Interfaces.Models;
using Orleans.Silo.States;

namespace Orleans.Silo.Grains
{
    public class ChatRoomGrain : Grain<ChatRoomState>, IChatRoomGrain
    {
        public async Task<bool> AddUserToChatRoom(string username)
        {
            if (!State.Usernames.Contains(username))
            {
                State.Usernames.Add(username);
                await WriteStateAsync();

                return true;
            }

            return false;
        }

        public Task<List<ChatMessage>> GetMessages(int count = 30)
        {
            return Task.FromResult(State.Messages
               .OrderByDescending(m => m.CreatedAt)
               .Take(count)
               .ToList());
        }

        public Task<HashSet<string>> GetUsers()
        {
            return Task.FromResult(State.Usernames);
        }

        public Task<ChatRoomState> GetState()
        {
            return Task.FromResult(State);
        }

        public async Task<bool> InitializeRoom(string username, string roomName)
        {
            State.Usernames.Add(username);
            State.CharRoomName = roomName;

            await WriteStateAsync();

            return true;
        }

        public Task<List<ChatMessage>> LoadOlderMessages(int skip, int take)
        {
            return Task.FromResult(State.Messages
                .OrderByDescending(m => m.CreatedAt)
                .Skip(skip)
                .Take(take)
                .ToList());
        }

        public async Task<bool> RemoveUserFromChatRoom(string username)
        {
            var isUserPersist = State.Usernames.Contains(username);
            if (isUserPersist)
            {
                State.Usernames.Remove(username);
                await WriteStateAsync();

                return true;
            }

            return false;
        }

        public async Task<bool> SendMessage(ChatMessage message)
        {
            if (message == null || string.IsNullOrWhiteSpace(message.Message))
            {
                return false;
            }

            State.Messages.Add(message);
            await WriteStateAsync();

            return true;
        }

    }
}
