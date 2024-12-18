using Orleans.Interfaces.Models;

namespace Orleans.Silo.States
{
    [GenerateSerializer]
    public class ChatRoomState
    {
        [Id(0)]
        public string CharRoomName { get; set; }

        [Id(1)]
        public HashSet<string> Usernames { get; set; } = new ();

        [Id(2)]
        public ICollection<ChatMessage> Messages { get; set; } = new List<ChatMessage>();
    }
}
