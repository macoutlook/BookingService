namespace Service.Dto;

public record ScheduletDto(
    Facility Facility,
    int SlotDurationMinutes,
    DaySchedule? Monday = null,
    DaySchedule? Tuesday = null,
    DaySchedule? Wednesday = null,
    DaySchedule? Thursday = null,
    DaySchedule? Friday = null
);

public record Facility(
    string Name,
    string Address
);

public record DaySchedule(
    WorkPeriod WorkPeriod,
    List<BusySlot> BusySlots
);

public record WorkPeriod(
    int StartHour,
    int EndHour,
    int LunchStartHour,
    int LunchEndHour
);

public record BusySlot(
    DateTime Start,
    DateTime End
);