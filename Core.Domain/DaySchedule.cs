namespace Core.Domain;

public sealed class DaySchedule
{
    public ulong Id { get; init; }
    public required DayOfWeek Day { get; init; }
    public required WorkPeriod WorkPeriod { get; init; } = null!;
}