﻿namespace Core.Domain;

public sealed class Schedule : Entity<ulong>
{
    public DateOnly StartDate { get; init; }
    public required Facility Facility { get; init; } = null!;
    public required uint SlotDurationMinutes { get; init; }
    public required IEnumerable<DaySchedule> DaySchedules { get; init; } = null!;
    public IEnumerable<Slot> BusySlots { get; init; } = null!;
}