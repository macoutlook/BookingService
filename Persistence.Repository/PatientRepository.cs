using Core.Persistence.Contract;
using Microsoft.EntityFrameworkCore;

namespace Persistence.AppointmentRepository;

public class PatientRepository(AppointmentContext context) : IPatientRepository
{
    public async Task<ulong?> GetPatientIdByEmail(string email, CancellationToken cancellationToken = default)
    {
       return (await context.Patient.AsNoTracking()
            .FirstOrDefaultAsync(p => p.Email.Equals(email), cancellationToken))?.Id;
    }
}