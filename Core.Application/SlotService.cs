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

        var closestMonday = GetClosestMonday(appointment.Slot.Start.Date);
        var schedule = await appointmentRepository.GetScheduleAsync(closestMonday, cancellationToken);
        if (schedule is null)
            throw new EntityNotFoundException("Slot cannot be found in any schedule.");

        appointment.Slot.ScheduleStartDate = closestMonday;

        return await appointmentRepository.AddAsync(appointment, cancellationToken);
    }

    public async Task<Schedule?> GetAvailabilityAsync(string date, CancellationToken cancellationToken = default)
    {
        var dateOnly = DateOnly.ParseExact(date, DateOnlyFormat, CultureInfo.InvariantCulture);
        return await appointmentRepository.GetScheduleAsync(dateOnly, cancellationToken);
    }

    private DateOnly GetClosestMonday(DateTime date)
    {
        var diff = date.DayOfWeek - DayOfWeek.Monday;
        return DateOnly.FromDateTime(date.AddDays(-diff));
    }
}