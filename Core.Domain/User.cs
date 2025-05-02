using System.ComponentModel.DataAnnotations;

namespace Core.Domain;

public class User
{
    public int Id { get; init; }
    [MaxLength(128, ErrorMessage = "Name must be 128 characters or less")]
    [MinLength(1)]
    public required string Name { get; init; }
    // TODO: Implement PBKDF2 password hashing
    public required string Password { get; init; }
}