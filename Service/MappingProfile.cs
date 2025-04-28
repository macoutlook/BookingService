using AutoMapper;
using Core.Domain;
using Service.Dto;
using Facility = Service.Dto.Facility;
using DomainFacility = Core.Domain.Facility;
using Patient = Service.Dto.Patient;
using DomainPatient = Core.Domain.Patient;
using WorkPeriod = Service.Dto.WorkPeriod;
using DomainWorkPeriod = Core.Domain.WorkPeriod;

namespace Service;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<AppointmentDto, Appointment>().ReverseMap();
        CreateMap<BusySlot, Slot>().ReverseMap();
        CreateMap<Facility, DomainFacility>().ReverseMap();
        CreateMap<Patient, DomainPatient>().ReverseMap();
        CreateMap<WorkPeriod, DomainWorkPeriod>().ReverseMap();
    }
}