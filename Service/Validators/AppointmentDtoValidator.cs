using System.Globalization;
using FluentValidation;
using Service.Dto;

namespace Service.Validators;

public sealed class AppointmentDtoValidator : AbstractValidator<AppointmentDto>
{
    private const string DateTimeFormat = "yyyy-MM-dd HH:mm:ss";

    public AppointmentDtoValidator()
    {
        RuleFor(a => a).NotEmpty();
        RuleFor(a => a).Custom((a, context) =>
        {
            var startDateTimeParsed = DateTime.TryParseExact(a.Start, DateTimeFormat, CultureInfo.InvariantCulture,
                DateTimeStyles.None, out var startDateTime);
            var endDateTimeParsed = DateTime.TryParseExact(a.End, DateTimeFormat, CultureInfo.InvariantCulture,
                DateTimeStyles.None, out var endDateTime);
            if (startDateTimeParsed && endDateTimeParsed)
            {
                if(startDateTime >= endDateTime)
                    context.AddFailure("Start date must be before end date.");
            }
        });

        RuleFor(a => a.Start)
            .Must(d => DateTime.TryParseExact(d, DateTimeFormat, CultureInfo.InvariantCulture, DateTimeStyles.None,
                out _))
            .WithMessage("Start date is not in the correct format.");
        RuleFor(a => a.End)
            .Must(d => DateTime.TryParseExact(d, DateTimeFormat, CultureInfo.InvariantCulture, DateTimeStyles.None,
                out _))
            .WithMessage("End date is not in the correct format.");


        RuleFor(a => a.Comments).MaximumLength(320);
        RuleFor(a => a.Patient).NotEmpty();
        RuleFor(a => a.Patient.Name).NotEmpty().Length(1, 50)
            .WithMessage("Name must be between 1 and 50 characters long.");
        RuleFor(a => a.Patient.SecondName).NotEmpty().Length(1, 50)
            .WithMessage("SecondName must be between 1 and 50 characters long.");
        RuleFor(a => a.Patient.Email).NotEmpty().Length(3, 255)
            .WithMessage("Email must be between 3 and 255 characters long.").EmailAddress()
            .WithMessage("Email is not valid.");
        // TODO: validate phone number
        RuleFor(a => a.Patient.Phone).NotEmpty().Length(3, 25)
            .WithMessage("Phone must be between 3 and 25 characters long.");
    }
}