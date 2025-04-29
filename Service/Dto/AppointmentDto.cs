namespace Service.Dto;

public sealed record AppointmentDto(
    DateTime Start,
    DateTime End,
    string Comments,
    PatientDto Patient
);

public sealed record PatientDto(
    string Name,
    string SecondName,
    string Email,
    string Phone
);