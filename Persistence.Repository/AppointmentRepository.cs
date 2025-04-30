using Core.Domain;
using Core.Persistence.Contract;
using Microsoft.EntityFrameworkCore;

namespace Persistence.AppointmentRepository;

public class AppointmentRepository(AppointmentContext context) : IAppointmentRepository
{
    # region Command

    public async Task<ulong> AddAsync(Appointment appointment, CancellationToken cancellationToken = default)
    {
        context.Patient.Attach(appointment.Patient);
        await context.Appointment.AddAsync(appointment, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);  
        
        return appointment.Id;
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