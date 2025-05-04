using Core.Domain;
using Core.Persistence.Contract;
using Microsoft.EntityFrameworkCore;

namespace Persistence.Repository;

public sealed class UserRepository(AppointmentContext context) : IUserRepository
{
    public async Task<User?> GetUserByName(string name, CancellationToken cancellationToken = default)
    {
        return await context.User
            .FirstOrDefaultAsync(p => p.Name.Equals(name), cancellationToken);
    }
}