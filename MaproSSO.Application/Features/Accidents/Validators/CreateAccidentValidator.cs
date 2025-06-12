using FluentValidation;
using MaproSSO.Application.Features.Accidents.Commands;

namespace MaproSSO.Application.Features.Accidents.Validators;

public class CreateAccidentValidator : AbstractValidator<CreateAccidentCommand>
{
    public CreateAccidentValidator()
    {
        RuleFor(x => x.Type)
            .NotEmpty().WithMessage("Type is required")
            .Must(x => new[] { "Accident", "Incident", "NearMiss" }.Contains(x))
            .WithMessage("Invalid accident type");

        RuleFor(x => x.Severity)
            .NotEmpty().WithMessage("Severity is required")
            .Must(x => new[] { "Minor", "Moderate", "Serious", "Fatal" }.Contains(x))
            .WithMessage("Invalid severity level");

        RuleFor(x => x.OccurredAt)
            .LessThanOrEqualTo(DateTime.Now)
            .WithMessage("Occurrence date cannot be in the future");

        RuleFor(x => x.Shift)
            .NotEmpty().WithMessage("Shift is required")
            .MaximumLength(50).WithMessage("Shift cannot exceed 50 characters");

        RuleFor(x => x.Location)
            .NotEmpty().WithMessage("Location is required")
            .MaximumLength(200).WithMessage("Location cannot exceed 200 characters");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Description is required")
            .MaximumLength(2000).WithMessage("Description cannot exceed 2000 characters");

        RuleFor(x => x.People)
            .Must(people => people.Any(p => p.PersonType == "Affected"))
            .WithMessage("At least one affected person must be specified");

        RuleForEach(x => x.People).SetValidator(new CreateAccidentPersonValidator());
    }
}

public class CreateAccidentPersonValidator : AbstractValidator<CreateAccidentPersonDto>
{
    public CreateAccidentPersonValidator()
    {
        RuleFor(x => x.PersonType)
            .NotEmpty().WithMessage("Person type is required")
            .Must(x => new[] { "Affected", "Responsible", "Witness" }.Contains(x))
            .WithMessage("Invalid person type");

        RuleFor(x => x.FullName)
            .NotEmpty().WithMessage("Full name is required")
            .MaximumLength(200).WithMessage("Full name cannot exceed 200 characters");

        RuleFor(x => x.Age)
            .InclusiveBetween(16, 100).When(x => x.Age.HasValue)
            .WithMessage("Age must be between 16 and 100");

        RuleFor(x => x.Gender)
            .Must(x => string.IsNullOrEmpty(x) || new[] { "M", "F", "Other" }.Contains(x))
            .WithMessage("Invalid gender");

        RuleFor(x => x.LostWorkDays)
            .GreaterThanOrEqualTo(0).When(x => x.LostWorkDays.HasValue)
            .WithMessage("Lost work days cannot be negative");
    }
}

public class UpdateAccidentValidator : AbstractValidator<UpdateAccidentCommand>
{
    public UpdateAccidentValidator()
    {
        RuleFor(x => x.AccidentId)
            .NotEmpty().WithMessage("Accident ID is required");

        RuleFor(x => x.Type)
            .NotEmpty().WithMessage("Type is required")
            .Must(x => new[] { "Accident", "Incident", "NearMiss" }.Contains(x))
            .WithMessage("Invalid accident type");

        RuleFor(x => x.Severity)
            .NotEmpty().WithMessage("Severity is required")
            .Must(x => new[] { "Minor", "Moderate", "Serious", "Fatal" }.Contains(x))
            .WithMessage("Invalid severity level");

        RuleFor(x => x.Status)
            .Must(x => new[] { "Reported", "UnderInvestigation", "Closed" }.Contains(x))
            .WithMessage("Invalid status");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Description is required")
            .MaximumLength(2000).WithMessage("Description cannot exceed 2000 characters");
    }
}

public class CloseInvestigationValidator : AbstractValidator<CloseInvestigationCommand>
{
    public CloseInvestigationValidator()
    {
        RuleFor(x => x.AccidentId)
            .NotEmpty().WithMessage("Accident ID is required");

        RuleFor(x => x.ImmediateCauses)
            .NotEmpty().WithMessage("Immediate causes are required")
            .MaximumLength(2000).WithMessage("Immediate causes cannot exceed 2000 characters");

        RuleFor(x => x.RootCauses)
            .NotEmpty().WithMessage("Root causes are required")
            .MaximumLength(2000).WithMessage("Root causes cannot exceed 2000 characters");
    }
}