using Core.Domain;

namespace Core.Application.Contract;

public interface ISlotService
{
    public Task<ulong> TakeSlotAsync(Appointment appointment, CancellationToken cancellationToken = default);
    public Task<Schedule?> GetAvailabilityAsync(string date, CancellationToken cancellationToken = default);
}