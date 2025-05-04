using System.Globalization;
using Core.Application.Contract;
using Core.Domain;
using Core.Exceptions;
using Core.Persistence.Contract;

namespace Core.Application;

public class SlotService(
    IAppointmentRepository appointmentRepository,
    IPatientRepository patientRepository)
    : ISlotService
{
    private const string DateOnlyFormat = "yyyyMMdd";

    public async Task<ulong> TakeSlotAsync(Appointment appointment, CancellationToken cancellationToken = default)
    {
        var patientId = await patientRepository.GetPatientIdByEmail(appointment.Patient.Email, cancellationToken);
        if (patientId is null)
            throw new EntityNotFoundException("Given patient cannot be found.");

        appointment.Patient.Id = patientId.Value;

        var currentWeekMonday = GetCurrentWeekMonday(appointment.Slot.Start.Date);
        var schedule = await appointmentRepository.GetScheduleAsync(currentWeekMonday, cancellationToken);
        if (schedule is null)
            throw new EntityNotFoundException("Schedule cannot be found.");

        appointment.Slot.ScheduleStartDate = currentWeekMonday;

        var daySchedule = schedule.DaySchedules.FirstOrDefault(d => d.Day.Equals(appointment.Slot.Day));
        if (daySchedule is null)
            throw new EntityNotFoundException("Day schedule for appointment day cannot be found.");

        if (!appointment.DoesAppointmentMatchSlotDuration(schedule.SlotDurationMinutes))
            throw new ScheduleException("Slot duration does not match schedule slot duration.");

        if (!appointment.DoesAppointmentMatchWorkPeriod(schedule.StartDate, daySchedule))
            throw new ScheduleException("Slot is out of working hours.");

        if (!appointment.DoesAppointmentMatchPlannedBreak(schedule.StartDate, daySchedule))
            throw new ScheduleException("Slot is during planned break.");

        if (appointment.DoesAppointmentOverlapExistingAppointments(schedule.BusySlots.ToList()))
            throw new ScheduleException("Slot is already taken.");

        return await appointmentRepository.AddAsync(appointment, cancellationToken);
    }

    public async Task<Schedule?> GetAvailabilityAsync(string date, CancellationToken cancellationToken = default)
    {
        var dateOnly = DateOnly.ParseExact(date, DateOnlyFormat, CultureInfo.InvariantCulture);
        return await appointmentRepository.GetScheduleAsync(dateOnly, cancellationToken);
    }

    private DateOnly GetCurrentWeekMonday(DateTime date)
    {
        var diff = date.DayOfWeek - DayOfWeek.Monday;
        return DateOnly.FromDateTime(date.AddDays(-diff));
    }
}