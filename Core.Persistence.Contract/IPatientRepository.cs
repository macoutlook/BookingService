namespace Core.Persistence.Contract;

public interface IPatientRepository 
{
    public Task<ulong?> GetPatientIdByEmail(string email, CancellationToken cancellationToken = default);
}