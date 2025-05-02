namespace Core.Domain;

public sealed class Slot
{
    public ulong Id { get; init; }
    public required DateTime Start { get; init; }
    public required DateTime End { get; init; }
    public required DayOfWeek Day { get; init; }
    public DateOnly ScheduleStartDate { get; set; }
}