using Core.Domain;

namespace Core.Persistence.Contract;

public interface IAppointmentRepository 
{
    public Task<ulong> AddAsync(Appointment appointment, CancellationToken cancellationToken = default);
    public Task<Schedule?> GetScheduleAsync(DateOnly date, CancellationToken cancellationToken = default);
}