using FluentValidation;

namespace MaproSSO.Application.Features.Tenants.Commands.CreateTenant
{
    public class CreateTenantCommandValidator : AbstractValidator<CreateTenantCommand>
    {
        public CreateTenantCommandValidator()
        {
            RuleFor(x => x.CompanyName)
                .NotEmpty().WithMessage("El nombre de la empresa es requerido")
                .MaximumLength(200).WithMessage("El nombre no puede exceder 200 caracteres");

            RuleFor(x => x.TaxId)
                .NotEmpty().WithMessage("El RUC/DNI es requerido")
                .Matches(@"^\d{11}$|^\d{8}$").WithMessage("El RUC debe tener 11 dígitos o DNI 8 dígitos");

            RuleFor(x => x.Industry)
                .NotEmpty().WithMessage("La industria es requerida")
                .MaximumLength(100);

            RuleFor(x => x.Phone)
                .NotEmpty().WithMessage("El teléfono es requerido")
                .Matches(@"^\+?[\d\s\-\(\)]+$").WithMessage("Formato de teléfono inválido");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("El email es requerido")
                .EmailAddress().WithMessage("Formato de email inválido");

            RuleFor(x => x.Address)
                .NotNull().WithMessage("La dirección es requerida")
                .SetValidator(new AddressDtoValidator());

            // Admin User validation
            RuleFor(x => x.AdminFirstName)
                .NotEmpty().WithMessage("El nombre del administrador es requerido");

            RuleFor(x => x.AdminLastName)
                .NotEmpty().WithMessage("El apellido del administrador es requerido");

            RuleFor(x => x.AdminEmail)
                .NotEmpty().WithMessage("El email del administrador es requerido")
                .EmailAddress().WithMessage("Formato de email inválido");

            RuleFor(x => x.AdminUsername)
                .NotEmpty().WithMessage("El usuario del administrador es requerido")
                .MinimumLength(4).WithMessage("El usuario debe tener al menos 4 caracteres")
                .Matches(@"^[a-zA-Z0-9_]+$").WithMessage("El usuario solo puede contener letras, números y guión bajo");

            RuleFor(x => x.AdminPassword)
                .NotEmpty().WithMessage("La contraseña es requerida")
                .MinimumLength(12).WithMessage("La contraseña debe tener al menos 12 caracteres");

            RuleFor(x => x.PlanId)
                .NotEmpty().WithMessage("El plan es requerido");

            RuleFor(x => x.BillingCycle)
                .NotEmpty().WithMessage("El ciclo de facturación es requerido")
                .Must(x => x == "Monthly" || x == "Annual").WithMessage("Ciclo de facturación inválido");
        }
    }

    public class AddressDtoValidator : AbstractValidator<AddressDto>
    {
        public AddressDtoValidator()
        {
            RuleFor(x => x.Country)
                .NotEmpty().WithMessage("El país es requerido");

            RuleFor(x => x.State)
                .NotEmpty().WithMessage("El departamento es requerido");

            RuleFor(x => x.City)
                .NotEmpty().WithMessage("La ciudad es requerida");

            RuleFor(x => x.Street)
                .NotEmpty().WithMessage("La dirección es requerida");
        }
    }
}