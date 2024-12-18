namespace Orleans.Interfaces.IGrains
{
    public interface IUserGrain : IGrainWithStringKey
    {
        Task DeactivateUser();

        Task<bool> IsActive();
    }
}
