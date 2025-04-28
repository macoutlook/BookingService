namespace Core.Domain;

public class Schedule
{
    public ulong Id { get; init; }
    public Facility Facility { get; init; } = null!;
    public int SlotDurationMinutes { get; init; }
    public IEnumerable<DaySchedule> DaySchedules { get; init; } = null!;
    public DateOnly StartDate { get; init; }
    public IEnumerable<Slot> BusySlots { get; init; } = null!;
}