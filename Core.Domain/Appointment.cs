using System.ComponentModel.DataAnnotations;

namespace Core.Domain;

public sealed class Appointment
{
    public ulong Id { get; init; }
    
    public required Slot Slot { get; init; }
    
    [MaxLength(320, ErrorMessage = "Comments must be 320 characters or less")]
    [MinLength(1)]
    public string? Comments { get; init; }
    
    public required Patient Patient { get; init; }
}