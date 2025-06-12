using System;
using FluentValidation;

namespace MaproSSO.Application.Features.SSO.Inspections.Commands.CreateInspection
{
    public class CreateInspectionCommandValidator : AbstractValidator<CreateInspectionCommand>
    {
        public CreateInspectionCommandValidator()
        {
            RuleFor(x => x.ProgramId)
                .NotEmpty().WithMessage("El programa es requerido");

            RuleFor(x => x.AreaId)
                .NotEmpty().WithMessage("El área es requerida");

            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("El título es requerido")
                .MaximumLength(200).WithMessage("El título no puede exceder 200 caracteres");

            RuleFor(x => x.InspectorUserId)
                .NotEmpty().WithMessage("El inspector es requerido");

            RuleFor(x => x.ScheduledDate)
                .GreaterThan(DateTime.UtcNow.Date)
                .WithMessage("La fecha programada debe ser futura");
        }
    }
}