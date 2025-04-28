using System.ComponentModel.DataAnnotations;

namespace Core.Domain;

public class Patient
{
    public ulong Id { get; init; }
    
    [MaxLength(50, ErrorMessage = "Name must be 50 characters or less")]
    [MinLength(1)]
    public required string Name { get; init; } = null!;
    
    [MaxLength(50, ErrorMessage = "Surname must be 50 characters or less")]
    [MinLength(1)]
    public required string Surname { get; init; } = null!;
    
    [MaxLength(255, ErrorMessage = "Email must be 255 characters at most and 3 characters at least")]
    [MinLength(3)]
    public required string Email { get; init; } = null!;
    
    [MaxLength(25, ErrorMessage = "Phone must be 25 characters at most and 3 characters at least")]
    [MinLength(3)]
    public required string Phone { get; init; } = null!;
}