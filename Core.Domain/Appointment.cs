using System.ComponentModel.DataAnnotations;

namespace Core.Domain;

public sealed class Appointment
{
    public ulong Id { get; init; }
    
    public required Slot Slot { get; init; }
    
    [MaxLength(320, ErrorMessage = "Comments must be 320 characters or less")]
    [MinLength(1)]
    public string? Comments { get; init; }
    
    public required Patient Patient { get; init; }

    public bool DoesAppointmentMatchSlotDuration(uint slotDurationInMinutes)
    {
        return Slot.End - Slot.Start == TimeSpan.FromMinutes(slotDurationInMinutes);
    }
    
    public bool DoesAppointmentMatchWorkPeriod(DateOnly scheduleStartDate, DaySchedule daySchedule)
    {
        var workDayDate = scheduleStartDate.AddDays((int)daySchedule.Day - 1);
        var startWorkDateTime =
            workDayDate
                .ToDateTime(new TimeOnly(daySchedule.WorkPeriod.StartHour, 0, 0));
        var endWorkDateTime =
            workDayDate
                .ToDateTime(new TimeOnly(daySchedule.WorkPeriod.EndHour, 0, 0));

        return Slot.Start >= startWorkDateTime && Slot.End <= endWorkDateTime;
    }
    
    public bool DoesAppointmentMatchPlannedBreak(DateOnly scheduleStartDate, DaySchedule daySchedule)
    {
        var workDayDate = scheduleStartDate.AddDays((int)daySchedule.Day - 1);
        var startLunchDateTime =
            workDayDate
                .ToDateTime(new TimeOnly(daySchedule.WorkPeriod.LunchStartHour, 0, 0));
        var endLunchDateTime =
            workDayDate
                .ToDateTime(new TimeOnly(daySchedule.WorkPeriod.LunchEndHour, 0, 0));

        return (Slot.Start <= startLunchDateTime || Slot.Start >= endLunchDateTime) &&
               (Slot.End <= startLunchDateTime || Slot.End >= endLunchDateTime);
    }
}