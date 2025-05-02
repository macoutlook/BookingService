namespace Service.Dto;

public record ScheduleDto(
    FacilityDto Facility,
    int SlotDurationMinutes,
    DayScheduleDto? Monday = null,
    DayScheduleDto? Tuesday = null,
    DayScheduleDto? Wednesday = null,
    DayScheduleDto? Thursday = null,
    DayScheduleDto? Friday = null
);

public record FacilityDto(
    string Name,
    string Address
);

public record DayScheduleDto(
    WorkPeriodDto WorkPeriod,
    List<SlotDto> BusySlots
);

public record WorkPeriodDto(
    int StartHour,
    int EndHour,
    int LunchStartHour,
    int LunchEndHour
);

public record SlotDto(
    DateTime Start,
    DateTime End
);