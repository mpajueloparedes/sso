using System;
using FluentValidation;

namespace MaproSSO.Application.Features.SSO.Announcements.Commands.AddCorrectiveAction
{
    public class AddCorrectiveActionCommandValidator : AbstractValidator<AddCorrectiveActionCommand>
    {
        public AddCorrectiveActionCommandValidator()
        {
            RuleFor(x => x.AnnouncementId)
                .NotEmpty().WithMessage("El ID del anuncio es requerido");

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("La descripción es requerida")
                .MaximumLength(2000).WithMessage("La descripción no puede exceder 2000 caracteres");

            RuleFor(x => x.ResponsibleUserId)
                .NotEmpty().WithMessage("El responsable es requerido");

            RuleFor(x => x.DueDate)
                .GreaterThan(DateTime.UtcNow.Date)
                .WithMessage("La fecha límite debe ser futura");
        }
    }
}