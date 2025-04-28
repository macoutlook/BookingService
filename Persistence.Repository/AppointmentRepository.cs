using Core.Domain;
using Core.Persistence.Contract;
using Microsoft.EntityFrameworkCore;

namespace Persistence.AppointmentRepository;

public class AppointmentRepository(AppointmentContext context) : IAppointmentRepository
{
    # region Command

    public Task<ulong> AddAsync(Appointment appointment, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
    # endregion

    # region Query
    
    public async Task<Schedule?> GetScheduleAsync(DateOnly date, CancellationToken cancellationToken = default)
    {
        return await context.Schedule
            .Include(s => s.Facility)
            .Include(s => s.DaySchedules)
            .ThenInclude(s => s.WorkPeriod)
            .Include(s => s.BusySlots)
            .AsSingleQuery()
            .FirstOrDefaultAsync(s => s.StartDate
                .Equals(date), cancellationToken);
    }
    
    # endregion
}