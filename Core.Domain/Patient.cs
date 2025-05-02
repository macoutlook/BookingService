using System.ComponentModel.DataAnnotations;

namespace Core.Domain;

public sealed class Patient
{
    public ulong Id { get; set; }
    
    [MaxLength(50, ErrorMessage = "Name must be 50 characters or less")]
    [MinLength(1)]
    public required string Name { get; init; } = null!;
    
    [MaxLength(50, ErrorMessage = "SecondName must be 50 characters or less")]
    [MinLength(1)]
    public required string SecondName { get; init; } = null!;
    
    [MaxLength(255, ErrorMessage = "Email must be 255 characters")]
    [MinLength(3, ErrorMessage = "Email must be at least 3 characters")]
    [EmailAddress]
    public required string Email { get; init; } = null!;
    
    [MaxLength(25, ErrorMessage = "Phone must be 25 characters")]
    [MinLength(3, ErrorMessage = "Phone must be at least 3 characters")]
    [Phone]
    public required string Phone { get; init; } = null!;
}