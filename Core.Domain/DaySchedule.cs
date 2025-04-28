namespace Core.Domain;

public class DaySchedule
{
    public ulong Id { get; init; }
    public DayOfWeek Day { get; init; }
    public WorkPeriod WorkPeriod { get; init; } = null!;
}