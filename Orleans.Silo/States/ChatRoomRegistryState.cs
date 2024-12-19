namespace Orleans.Silo.States
{
    [GenerateSerializer]
    public class ChatRoomRegistryState
    {
        [Id(0)]
        public Dictionary<string, string> ChatRooms { get; set; } = new ();
    }
}
