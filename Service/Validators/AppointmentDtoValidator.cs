using FluentValidation;
using Service.Dto;

namespace Service.Validators;

internal abstract class AppointmentDtoValidator : AbstractValidator<AppointmentDto>
{
    public AppointmentDtoValidator()
    {
        // TODO: validate if date in get slots is monday
        
        // RuleFor(a => a).NotEmpty();
        // RuleFor(r => r.Title).NotEmpty();
        // RuleFor(r => r.Title).Length(1, 320).WithMessage("Title must be between 3 and 50 characters long.");
        // RuleFor(r => r.Author).NotEmpty();
        // RuleFor(r => r.Title).Length(3, 320).WithMessage("Title must be between 3 and 50 characters long.");
        // RuleFor(r => r.Isbn).NotEmpty();
        // RuleFor(r => r.Isbn).Length(17);
        // RuleFor(r => r.BookStatus).NotEmpty();
    }
}