namespace Core.Domain;

public sealed class Schedule
{
    public DateOnly StartDate { get; init; }
    public required Facility Facility { get; init; } = null!;
    public required int SlotDurationMinutes { get; init; }
    public required IEnumerable<DaySchedule> DaySchedules { get; init; } = null!;
    public IEnumerable<Slot> BusySlots { get; init; } = null!;
}