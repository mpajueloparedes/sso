using FluentValidation;
using MaproSSO.Application.Features.Trainings.Commands;

namespace MaproSSO.Application.Features.Trainings.Validators;

public class CreateTrainingValidator : AbstractValidator<CreateTrainingCommand>
{
    public CreateTrainingValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required")
            .MaximumLength(200).WithMessage("Title cannot exceed 200 characters");

        RuleFor(x => x.TrainingType)
            .NotEmpty().WithMessage("Training type is required")
            .Must(x => new[] { "Induction", "Periodic", "Specific", "Emergency" }.Contains(x))
            .WithMessage("Invalid training type");

        RuleFor(x => x.Mode)
            .NotEmpty().WithMessage("Mode is required")
            .Must(x => new[] { "Presential", "Virtual", "Mixed" }.Contains(x))
            .WithMessage("Invalid training mode");

        RuleFor(x => x.AreaId)
            .NotEmpty().WithMessage("Area ID is required");

        RuleFor(x => x.ScheduledDate)
            .GreaterThan(DateTime.Now)
            .WithMessage("Scheduled date must be in the future");

        RuleFor(x => x.Duration)
            .GreaterThan(0)
            .WithMessage("Duration must be greater than 0 minutes");

        RuleFor(x => x.MaxParticipants)
            .GreaterThan(0).When(x => x.MaxParticipants.HasValue)
            .WithMessage("Maximum participants must be greater than 0");

        RuleFor(x => x.Description)
            .MaximumLength(2000).WithMessage("Description cannot exceed 2000 characters");

        RuleFor(x => x.ExternalInstructor)
            .MaximumLength(200).WithMessage("External instructor name cannot exceed 200 characters");

        RuleFor(x => x.Location)
            .MaximumLength(200).WithMessage("Location cannot exceed 200 characters");

        // Validation: Either internal instructor or external instructor must be specified
        RuleFor(x => x)
            .Must(x => x.InstructorUserId.HasValue || !string.IsNullOrEmpty(x.ExternalInstructor))
            .WithMessage("Either an internal instructor or external instructor must be specified");
    }
}

public class UpdateTrainingValidator : AbstractValidator<UpdateTrainingCommand>
{
    public UpdateTrainingValidator()
    {
        RuleFor(x => x.TrainingId)
            .NotEmpty().WithMessage("Training ID is required");

        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required")
            .MaximumLength(200).WithMessage("Title cannot exceed 200 characters");

        RuleFor(x => x.TrainingType)
            .NotEmpty().WithMessage("Training type is required")
            .Must(x => new[] { "Induction", "Periodic", "Specific", "Emergency" }.Contains(x))
            .WithMessage("Invalid training type");

        RuleFor(x => x.Mode)
            .NotEmpty().WithMessage("Mode is required")
            .Must(x => new[] { "Presential", "Virtual", "Mixed" }.Contains(x))
            .WithMessage("Invalid training mode");

        RuleFor(x => x.Duration)
            .GreaterThan(0)
            .WithMessage("Duration must be greater than 0 minutes");

        RuleFor(x => x.Status)
            .Must(x => new[] { "Scheduled", "InProgress", "Completed", "Cancelled" }.Contains(x))
            .WithMessage("Invalid status");

        RuleFor(x => x.MaxParticipants)
            .GreaterThan(0).When(x => x.MaxParticipants.HasValue)
            .WithMessage("Maximum participants must be greater than 0");
    }
}

public class RegisterParticipantValidator : AbstractValidator<RegisterParticipantCommand>
{
    public RegisterParticipantValidator()
    {
        RuleFor(x => x.TrainingId)
            .NotEmpty().WithMessage("Training ID is required");

        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("User ID is required");
    }
}

public class TakeAttendanceValidator : AbstractValidator<TakeAttendanceCommand>
{
    public TakeAttendanceValidator()
    {
        RuleFor(x => x.TrainingId)
            .NotEmpty().WithMessage("Training ID is required");

        RuleFor(x => x.AttendanceRecords)
            .NotEmpty().WithMessage("Attendance records are required");

        RuleForEach(x => x.AttendanceRecords).SetValidator(new AttendanceRecordValidator());
    }
}

public class AttendanceRecordValidator : AbstractValidator<AttendanceRecordDto>
{
    public AttendanceRecordValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("User ID is required");

        RuleFor(x => x.AttendanceStatus)
            .NotEmpty().WithMessage("Attendance status is required")
            .Must(x => new[] { "Present", "Absent", "Excused" }.Contains(x))
            .WithMessage("Invalid attendance status");

        RuleFor(x => x.Score)
            .InclusiveBetween(0, 100).When(x => x.Score.HasValue)
            .WithMessage("Score must be between 0 and 100");

        RuleFor(x => x.Comments)
            .MaximumLength(500).WithMessage("Comments cannot exceed 500 characters");
    }
}