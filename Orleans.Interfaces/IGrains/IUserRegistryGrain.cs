
namespace Orleans.Interfaces.IGrains
{
    public interface IUserRegistryGrain : IGrainWithIntegerKey
    {
        Task<bool> RegisterUser(string username);

        Task RemoveUser(string username);

        Task<HashSet<string>> GetUsers();
    }
}
