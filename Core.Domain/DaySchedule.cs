namespace Core.Domain;

public class DaySchedule
{
    public ulong Id { get; init; }
    public required DayOfWeek Day { get; init; }
    public required WorkPeriod WorkPeriod { get; init; } = null!;
}