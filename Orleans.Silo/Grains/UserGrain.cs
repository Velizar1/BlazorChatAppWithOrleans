using Orleans.Interfaces.IGrains;
using Orleans.Silo.States;

namespace Orleans.Silo.Grains
{
    public class UserGrain : Grain<UserState>, IUserGrain
    {
        public Task DeactivateUser()
        {
            State.IsActive = false;
            return Task.CompletedTask;
        }

        public Task<bool> IsActive()
        {
            return Task.FromResult(State.IsActive);
        }
    }
}
