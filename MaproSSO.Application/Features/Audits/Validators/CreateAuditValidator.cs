using FluentValidation;
using MaproSSO.Application.Features.Audits.Commands;

namespace MaproSSO.Application.Features.Audits.Validators;

public class CreateAuditValidator : AbstractValidator<CreateAuditCommand>
{
    public CreateAuditValidator()
    {
        RuleFor(x => x.ProgramId)
            .NotEmpty().WithMessage("Program ID is required");

        RuleFor(x => x.AreaId)
            .NotEmpty().WithMessage("Area ID is required");

        RuleFor(x => x.AuditType)
            .NotEmpty().WithMessage("Audit type is required")
            .Must(x => new[] { "Internal", "External", "Certification" }.Contains(x))
            .WithMessage("Invalid audit type");

        RuleFor(x => x.AuditorUserId)
            .NotEmpty().WithMessage("Auditor is required");

        RuleFor(x => x.ScheduledDate)
            .GreaterThan(DateTime.Today.AddDays(-1))
            .WithMessage("Scheduled date cannot be in the past");
    }
}

public class EvaluateAuditCriteriaValidator : AbstractValidator<EvaluateAuditCriteriaCommand>
{
    public EvaluateAuditCriteriaValidator()
    {
        RuleFor(x => x.AuditId)
            .NotEmpty().WithMessage("Audit ID is required");

        RuleFor(x => x.CriteriaId)
            .NotEmpty().WithMessage("Criteria ID is required");

        RuleFor(x => x.Score)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Score must be greater than or equal to 0");
    }
}