using System.Globalization;
using AutoMapper;
using Core.Domain;
using Service.Dto;
using DomainFacility = Core.Domain.Facility;
using DomainPatient = Core.Domain.Patient;
using DomainWorkPeriod = Core.Domain.WorkPeriod;

namespace Service;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<string, DateTime>().ConvertUsing(s => DateTime.Parse(s, CultureInfo.InvariantCulture));
        
        CreateMap<AppointmentDto, Appointment>()
            .ForMember(dest => dest.Slot, opt => opt.MapFrom(src => new Slot
            {
                Start = DateTime.Parse(src.Start),
                End = DateTime.Parse(src.End),
                Day = DateTime.Parse(src.Start).DayOfWeek
            }));
        
        CreateMap<Appointment, AppointmentDto>()
            .ForMember(dest => dest.Start,
                opt => opt.MapFrom(src => src.Slot.Start.ToString("yyyy-MM-dd HH:mm:ss")))
            .ForMember(dest => dest.End,
                opt => opt.MapFrom(src => src.Slot.End.ToString("yyyy-MM-dd HH:mm:ss")));

        CreateMap<SlotDto, Slot>().ReverseMap();
        CreateMap<FacilityDto, DomainFacility>().ReverseMap();
        CreateMap<PatientDto, DomainPatient>().ReverseMap();
        CreateMap<WorkPeriodDto, DomainWorkPeriod>().ReverseMap();
    }
}