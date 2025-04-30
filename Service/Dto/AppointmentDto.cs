namespace Service.Dto;

public sealed class AppointmentDto
{
    public required string Start { get; set; }
    public required string End { get; set; }
    public string? Comments { get; set; }
    public required PatientDto Patient { get; set; }
}

public sealed class PatientDto
{
    public required string Name { get; set; }
    public required string SecondName { get; set; }
    public required string Email { get; set; }
    public required string Phone { get; set; }
}
