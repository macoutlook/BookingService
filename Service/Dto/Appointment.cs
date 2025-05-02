namespace Service.Dto;

public sealed record AppointmentDto
{
    public required string Start { get; init; }
    public required string End { get; init; }
    public string? Comments { get; init; }
    public required PatientDto Patient { get; init; }
}

public sealed record PatientDto
{
    public required string Name { get; init; }
    public required string SecondName { get; init; }
    public required string Email { get; init; }
    public required string Phone { get; init; }
}