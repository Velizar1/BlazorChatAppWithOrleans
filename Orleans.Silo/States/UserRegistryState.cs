namespace Orleans.Silo.States
{
    [GenerateSerializer]
    public class UserRegistryState
    {
        [Id(0)]
        public HashSet<string> Usernames { get; set; } = new();
    }
}
