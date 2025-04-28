using AutoMapper;
using Core.Domain;
using Service.Dto;
using DaySchedule = Service.Dto.DaySchedule;
using Facility = Service.Dto.Facility;
using WorkPeriod = Service.Dto.WorkPeriod;

namespace Service.Adapters;

public class ScheduleAdapter(IMapper mapper)
{
    public ScheduletDto Adapt(Schedule schedule)
    {
        var monday = GetDaySchedule(schedule, DayOfWeek.Monday);
        var tuesday = GetDaySchedule(schedule, DayOfWeek.Tuesday);
        var wednesday = GetDaySchedule(schedule, DayOfWeek.Wednesday);
        var thursday = GetDaySchedule(schedule, DayOfWeek.Thursday);
        var friday = GetDaySchedule(schedule, DayOfWeek.Friday);

        var scheduleDto = new ScheduletDto(mapper.Map<Facility>(schedule.Facility), schedule.SlotDurationMinutes,
            monday, tuesday, wednesday, thursday, friday);
        return scheduleDto;
    }

    private DaySchedule GetDaySchedule(Schedule schedule, DayOfWeek dayOfWeek)
    {
        return new DaySchedule(
            mapper.Map<WorkPeriod>(schedule.DaySchedules.FirstOrDefault(d => d.Day.Equals(dayOfWeek))?.WorkPeriod),
            mapper.Map<List<BusySlot>>(schedule.BusySlots.Where(s => s.Day.Equals(dayOfWeek))));
    }
}