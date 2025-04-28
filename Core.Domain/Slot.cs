namespace Core.Domain;

public class Slot
{
    public ulong Id { get; init; }
    public DateTime Start { get; init; }
    public DateTime End { get; init; }
    public DayOfWeek Day { get; init; }
}