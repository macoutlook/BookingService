namespace Core.Domain;

public sealed class WorkPeriod : Entity<ulong>
{
    public required int StartHour { get; init; }
    public required int EndHour { get; init; }
    public int LunchStartHour { get; init; }
    public int LunchEndHour { get; init; }
}