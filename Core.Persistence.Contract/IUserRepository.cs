using Core.Domain;

namespace Core.Persistence.Contract;

public interface IUserRepository 
{
    public Task<User?> GetUserByName(string name, CancellationToken cancellationToken = default);
}