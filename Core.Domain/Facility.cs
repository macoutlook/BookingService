using System.ComponentModel.DataAnnotations;

namespace Core.Domain;

public sealed class Facility : Entity<ulong>
{
    [MaxLength(255, ErrorMessage = "Name must be 255 characters or less")]
    [MinLength(1)]
    public required string Name { get; init; } = null!;
    
    // TODO: Address should be represented in separate table which contains Street, Premise, City, Postal Code, Country represented by ISO 3166-1 alpha-2 country code
    [MaxLength(255, ErrorMessage = "Address must be 255 characters or less")]
    [MinLength(1)]
    public required string Address { get; init; } = null!;
}