namespace Core.Domain;

public class WorkPeriod
{
    public ulong Id { get; init; }
    public int StartHour { get; init; }
    public int EndHour { get; init; }
    public int LunchStartHour { get; init; }
    public int LunchEndHour { get; init; }
}