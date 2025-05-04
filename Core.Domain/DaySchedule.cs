namespace Core.Domain;

public sealed class DaySchedule : Entity<ulong>
{
    public required DayOfWeek Day { get; init; }
    public required WorkPeriod WorkPeriod { get; init; } = null!;
}