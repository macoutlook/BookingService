namespace Service.Dto;

public sealed record AppointmentDto(
    DateTime Start,
    DateTime End,
    string Comments,
    Patient Patient
);

public sealed record Patient(
    string Name,
    string SecondName,
    string Email,
    string Phone
);