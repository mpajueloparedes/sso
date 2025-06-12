using FluentValidation;

namespace MaproSSO.Application.Features.Tenants.Commands.UpdateTenant
{
    public class UpdateTenantCommandValidator : AbstractValidator<UpdateTenantCommand>
    {
        public UpdateTenantCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("El ID del tenant es requerido");

            RuleFor(x => x.CompanyName)
                .NotEmpty().WithMessage("El nombre de la empresa es requerido")
                .MaximumLength(200).WithMessage("El nombre no puede exceder 200 caracteres");

            RuleFor(x => x.Industry)
                .NotEmpty().WithMessage("La industria es requerida")
                .MaximumLength(100);

            RuleFor(x => x.Phone)
                .NotEmpty().WithMessage("El teléfono es requerido")
                .Matches(@"^\+?[\d\s\-\(\)]+$").WithMessage("Formato de teléfono inválido");

            RuleFor(x => x.Address)
                .NotNull().WithMessage("La dirección es requerida");

            RuleFor(x => x.EmployeeCount)
                .GreaterThanOrEqualTo(0).WithMessage("El número de empleados no puede ser negativo");
        }
    }
}