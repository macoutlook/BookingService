using System.Globalization;
using Core.Application.Contract;
using Core.Domain;
using Core.Persistence.Contract;

namespace Core.Application;

public class SlotService(
    IAppointmentRepository repository)
    : ISlotService
{
    private const string DateOnlyFormat = "yyyyMMdd";
    
    public Task<ulong> TakeSlotAsync(Slot slot, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task<Schedule?> GetAvailabilityAsync(string date, CancellationToken cancellationToken = default)
    {
        var dateOnly = DateOnly.ParseExact(date, DateOnlyFormat, CultureInfo.InvariantCulture);
        return await repository.GetScheduleAsync(dateOnly, cancellationToken);
    }
}