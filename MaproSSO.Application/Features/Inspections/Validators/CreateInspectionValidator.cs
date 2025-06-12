using FluentValidation;
using MaproSSO.Application.Features.Inspections.Commands;

namespace MaproSSO.Application.Features.Inspections.Validators;

public class CreateInspectionValidator : AbstractValidator<CreateInspectionCommand>
{
    public CreateInspectionValidator()
    {
        RuleFor(x => x.ProgramId)
            .NotEmpty().WithMessage("Program ID is required");

        RuleFor(x => x.AreaId)
            .NotEmpty().WithMessage("Area ID is required");

        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required")
            .MaximumLength(200).WithMessage("Title cannot exceed 200 characters");

        RuleFor(x => x.InspectorUserId)
            .NotEmpty().WithMessage("Inspector is required");

        RuleFor(x => x.ScheduledDate)
            .GreaterThan(DateTime.Today.AddDays(-1))
            .WithMessage("Scheduled date cannot be in the past");

        RuleFor(x => x.Description)
            .MaximumLength(2000).WithMessage("Description cannot exceed 2000 characters");
    }
}

public class UpdateInspectionValidator : AbstractValidator<UpdateInspectionCommand>
{
    public UpdateInspectionValidator()
    {
        RuleFor(x => x.InspectionId)
            .NotEmpty().WithMessage("Inspection ID is required");

        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required")
            .MaximumLength(200).WithMessage("Title cannot exceed 200 characters");

        RuleFor(x => x.InspectorUserId)
            .NotEmpty().WithMessage("Inspector is required");

        RuleFor(x => x.Status)
            .Must(x => new[] { "Pending", "InProgress", "Completed" }.Contains(x))
            .WithMessage("Invalid status");

        RuleFor(x => x.CompletionPercentage)
            .InclusiveBetween(0, 100)
            .WithMessage("Completion percentage must be between 0 and 100");
    }
}

public class CreateObservationValidator : AbstractValidator<CreateObservationCommand>
{
    public CreateObservationValidator()
    {
        RuleFor(x => x.InspectionId)
            .NotEmpty().WithMessage("Inspection ID is required");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Description is required")
            .MaximumLength(2000).WithMessage("Description cannot exceed 2000 characters");

        RuleFor(x => x.Type)
            .Must(x => new[] { "Safety", "Quality", "Environment", "Compliance" }.Contains(x))
            .WithMessage("Invalid observation type");

        RuleFor(x => x.Severity)
            .Must(x => new[] { "Low", "Medium", "High", "Critical" }.Contains(x))
            .WithMessage("Invalid severity level");

        RuleFor(x => x.ResponsibleUserId)
            .NotEmpty().WithMessage("Responsible user is required");

        RuleFor(x => x.DueDate)
            .GreaterThan(DateTime.Today)
            .WithMessage("Due date must be in the future");
    }
}