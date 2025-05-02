using FluentValidation;
using FluentValidation.Results;

namespace Service.Validators;

public sealed class ScheduleStartDateValidator
{
    private const string DateOnlyFormat = "yyyyMMdd";

    internal ValidationResult ValidateScheduleDate(string date)
    {
        var validator = new InlineValidator<string>();

        validator.RuleFor(s => s)
            .NotEmpty().WithMessage("Schedule date cannot be empty.")
            .Length(8).WithMessage("Date string must be 8 characters long.")
            .Must(d => DateOnly.TryParseExact(d, DateOnlyFormat, out _))
            .WithMessage("Given date is not in the correct format.")
            .Must(d => DateOnly.TryParseExact(d, DateOnlyFormat, out var dateOnly) &&
                       dateOnly.DayOfWeek is DayOfWeek.Monday)
            .WithMessage("Given date is not a Monday.");

        return validator.Validate(date);
    }
}