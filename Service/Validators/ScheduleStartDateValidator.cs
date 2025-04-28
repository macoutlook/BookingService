using FluentValidation;
using FluentValidation.Results;

namespace Service.Validators;

public class ScheduleStartDateValidator
{
    private const string DateOnlyFormat = "yyyyMMdd";
    
    public ValidationResult ValidateScheduleDate(string date)
    {
        var validator = new InlineValidator<string>();
        validator.RuleFor(s => s)
            .NotEmpty().WithMessage("Schedule date cannot be empty.")
            .Length(8).WithMessage("The string must be 8 characters long.");

        validator.RuleFor(s => s).Must(d => DateOnly.TryParseExact(d, DateOnlyFormat, out _)).WithMessage("Given date is not in the correct format.");
        validator.RuleFor(s => s).Must(d => DateOnly.TryParseExact(d, DateOnlyFormat, out var dateOnly) && dateOnly.DayOfWeek == DayOfWeek.Monday).WithMessage("Given date is not a Monday.");
           
       return validator.Validate(date);
    }
}