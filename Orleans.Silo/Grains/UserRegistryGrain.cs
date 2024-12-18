using Orleans.Interfaces.IGrains;
using Orleans.Silo.States;

namespace Orleans.Silo.Grains
{
    public class UserRegistryGrain : Grain<UserRegistryState>, IUserRegistryGrain
    {

        public Task<HashSet<string>> GetUsers()
        {
            return Task.FromResult(State.Usernames);
        }


        public async Task<bool> RegisterUser(string username)
        {
            if (!State.Usernames.Contains(username))
            {
                var userGrain = this.GrainFactory.GetGrain<IUserGrain>(username);

                var isUserActive = await userGrain.IsActive();
                //Username exists and was deactivated
                if (!isUserActive)
                {
                    return false;
                }

                State.Usernames.Add(username);
                await WriteStateAsync();

                return true;
            }

            return false;
        }

        public async Task RemoveUser(string username)
        {
            if (State.Usernames.Contains(username))
            {
                var userGrain = this.GrainFactory.GetGrain<IUserGrain>(username);
                await userGrain.DeactivateUser();

                State.Usernames.Remove(username);
                await WriteStateAsync();
            }
        }
    }
}
