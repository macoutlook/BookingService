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
        CreateMap<AppointmentDto, Appointment>().ReverseMap();
        CreateMap<SlotDto, Slot>().ReverseMap();
        CreateMap<FacilityDto, DomainFacility>().ReverseMap();
        CreateMap<PatientDto, DomainPatient>().ReverseMap();
        CreateMap<WorkPeriodDto, DomainWorkPeriod>().ReverseMap();
    }
}