using FluentValidation;
using MaproSSO.Application.Features.Audit.Commands;
using MaproSSO.Application.Features.Audit.Queries;

namespace MaproSSO.Application.Features.Audit.Validators;

public class CreateAuditLogValidator : AbstractValidator<CreateAuditLogCommand>
{
    public CreateAuditLogValidator()
    {
        RuleFor(x => x.Action)
            .NotEmpty().WithMessage("Action is required")
            .MaximumLength(100).WithMessage("Action cannot exceed 100 characters");

        RuleFor(x => x.EntityType)
            .NotEmpty().WithMessage("Entity type is required")
            .MaximumLength(100).WithMessage("Entity type cannot exceed 100 characters");

        RuleFor(x => x.EntityId)
            .MaximumLength(100).WithMessage("Entity ID cannot exceed 100 characters");

        RuleFor(x => x.UserName)
            .MaximumLength(100).WithMessage("User name cannot exceed 100 characters");

        RuleFor(x => x.IpAddress)
            .MaximumLength(50).WithMessage("IP address cannot exceed 50 characters");

        RuleFor(x => x.UserAgent)
            .MaximumLength(500).WithMessage("User agent cannot exceed 500 characters");

        RuleFor(x => x.Duration)
            .GreaterThanOrEqualTo(0).When(x => x.Duration.HasValue)
            .WithMessage("Duration must be greater than or equal to 0");
    }
}

public class CreateAccessLogValidator : AbstractValidator<CreateAccessLogCommand>
{
    public CreateAccessLogValidator()
    {
        RuleFor(x => x.Action)
            .NotEmpty().WithMessage("Action is required")
            .MaximumLength(50).WithMessage("Action cannot exceed 50 characters");

        RuleFor(x => x.UserName)
            .MaximumLength(100).WithMessage("User name cannot exceed 100 characters");

        RuleFor(x => x.IpAddress)
            .MaximumLength(50).WithMessage("IP address cannot exceed 50 characters");

        RuleFor(x => x.Location)
            .MaximumLength(200).WithMessage("Location cannot exceed 200 characters");

        RuleFor(x => x.DeviceInfo)
            .MaximumLength(500).WithMessage("Device info cannot exceed 500 characters");

        RuleFor(x => x.Browser)
            .MaximumLength(100).WithMessage("Browser cannot exceed 100 characters");

        RuleFor(x => x.OperatingSystem)
            .MaximumLength(100).WithMessage("Operating system cannot exceed 100 characters");

        RuleFor(x => x.FailureReason)
            .MaximumLength(500).WithMessage("Failure reason cannot exceed 500 characters");
    }
}

public class GetAuditLogsValidator : AbstractValidator<GetAuditLogsQuery>
{
    public GetAuditLogsValidator()
    {
        RuleFor(x => x.Page)
            .GreaterThan(0).WithMessage("Page must be greater than 0");

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100).WithMessage("Page size must be between 1 and 100");

        RuleFor(x => x.FromDate)
            .LessThanOrEqualTo(x => x.ToDate).When(x => x.FromDate.HasValue && x.ToDate.HasValue)
            .WithMessage("From date must be less than or equal to To date");
    }
}

public class GetAccessLogsValidator : AbstractValidator<GetAccessLogsQuery>
{
    public GetAccessLogsValidator()
    {
        RuleFor(x => x.Page)
            .GreaterThan(0).WithMessage("Page must be greater than 0");

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100).WithMessage("Page size must be between 1 and 100");

        RuleFor(x => x.FromDate)
            .LessThanOrEqualTo(x => x.ToDate).When(x => x.FromDate.HasValue && x.ToDate.HasValue)
            .WithMessage("From date must be less than or equal to To date");
    }
}