using Orleans.Interfaces.Models;

namespace Orleans.Silo.States
{
    [GenerateSerializer]
    public class UserState
    {
        [Id(0)]
        public string Username { get; set; }

        [Id(1)]
        public bool IsActive { get; set; } = true;
    }
}
