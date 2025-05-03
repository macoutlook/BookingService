using AutoMapper;
using Service.Dto;

namespace Service.Adapters;

public sealed class ScheduleAdapter(IMapper mapper)
{
    public ScheduleDto Adapt(Core.Domain.Schedule schedule)
    {
        var monday = GetDaySchedule(schedule, DayOfWeek.Monday);
        var tuesday = GetDaySchedule(schedule, DayOfWeek.Tuesday);
        var wednesday = GetDaySchedule(schedule, DayOfWeek.Wednesday);
        var thursday = GetDaySchedule(schedule, DayOfWeek.Thursday);
        var friday = GetDaySchedule(schedule, DayOfWeek.Friday);

        var scheduleDto = new ScheduleDto(mapper.Map<FacilityDto>(schedule.Facility), (int)schedule.SlotDurationMinutes,
            monday, tuesday, wednesday, thursday, friday);
        return scheduleDto;
    }

    private DayScheduleDto GetDaySchedule(Core.Domain.Schedule schedule, DayOfWeek dayOfWeek)
    {
        return new DayScheduleDto(
            mapper.Map<WorkPeriodDto>(schedule.DaySchedules.FirstOrDefault(d => d.Day.Equals(dayOfWeek))?.WorkPeriod),
            mapper.Map<List<SlotDto>>(schedule.BusySlots.Where(s => s.Day.Equals(dayOfWeek))));
    }
}