using FluentValidation;
using MaproSSO.Application.Features.Tenants.Commands;

namespace MaproSSO.Application.Features.Tenants.Validators;

public class CreateTenantValidator : AbstractValidator<CreateTenantCommand>
{
    public CreateTenantValidator()
    {
        RuleFor(x => x.CompanyName)
            .NotEmpty().WithMessage("Company name is required")
            .MaximumLength(200).WithMessage("Company name cannot exceed 200 characters");

        RuleFor(x => x.TaxId)
            .NotEmpty().WithMessage("Tax ID is required")
            .MaximumLength(20).WithMessage("Tax ID cannot exceed 20 characters");

        RuleFor(x => x.Industry)
            .NotEmpty().WithMessage("Industry is required")
            .MaximumLength(100).WithMessage("Industry cannot exceed 100 characters");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Invalid email format")
            .MaximumLength(200).WithMessage("Email cannot exceed 200 characters");

        RuleFor(x => x.Phone)
            .NotEmpty().WithMessage("Phone is required")
            .MaximumLength(50).WithMessage("Phone cannot exceed 50 characters");

        RuleFor(x => x.Country)
            .NotEmpty().WithMessage("Country is required")
            .MaximumLength(100).WithMessage("Country cannot exceed 100 characters");

        RuleFor(x => x.State)
            .NotEmpty().WithMessage("State is required")
            .MaximumLength(100).WithMessage("State cannot exceed 100 characters");

        RuleFor(x => x.City)
            .NotEmpty().WithMessage("City is required")
            .MaximumLength(100).WithMessage("City cannot exceed 100 characters");

        RuleFor(x => x.Address)
            .NotEmpty().WithMessage("Address is required")
            .MaximumLength(500).WithMessage("Address cannot exceed 500 characters");

        RuleFor(x => x.EmployeeCount)
            .GreaterThan(0).WithMessage("Employee count must be greater than 0");

        // Admin user validation
        RuleFor(x => x.AdminFirstName)
            .NotEmpty().WithMessage("Admin first name is required")
            .MaximumLength(100).WithMessage("Admin first name cannot exceed 100 characters");

        RuleFor(x => x.AdminLastName)
            .NotEmpty().WithMessage("Admin last name is required")
            .MaximumLength(100).WithMessage("Admin last name cannot exceed 100 characters");

        RuleFor(x => x.AdminEmail)
            .NotEmpty().WithMessage("Admin email is required")
            .EmailAddress().WithMessage("Invalid admin email format");

        RuleFor(x => x.AdminUsername)
            .NotEmpty().WithMessage("Admin username is required")
            .MinimumLength(3).WithMessage("Admin username must be at least 3 characters")
            .MaximumLength(100).WithMessage("Admin username cannot exceed 100 characters");

        RuleFor(x => x.AdminPassword)
            .NotEmpty().WithMessage("Admin password is required")
            .MinimumLength(8).WithMessage("Admin password must be at least 8 characters")
            .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,}$")
            .WithMessage("Admin password must contain at least one uppercase letter, one lowercase letter, one number, and one special character");

        // Optional fields validation
        RuleFor(x => x.Website)
            .Must(BeAValidUrl).When(x => !string.IsNullOrEmpty(x.Website))
            .WithMessage("Invalid website URL");
    }

    private bool BeAValidUrl(string? url)
    {
        if (string.IsNullOrEmpty(url))
            return true;

        return Uri.TryCreate(url, UriKind.Absolute, out var uriResult) &&
               (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
    }
}

public class UpdateTenantValidator : AbstractValidator<UpdateTenantCommand>
{
    public UpdateTenantValidator()
    {
        RuleFor(x => x.TenantId)
            .NotEmpty().WithMessage("Tenant ID is required");

        RuleFor(x => x.CompanyName)
            .NotEmpty().WithMessage("Company name is required")
            .MaximumLength(200).WithMessage("Company name cannot exceed 200 characters");

        RuleFor(x => x.Industry)
            .NotEmpty().WithMessage("Industry is required")
            .MaximumLength(100).WithMessage("Industry cannot exceed 100 characters");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Invalid email format")
            .MaximumLength(200).WithMessage("Email cannot exceed 200 characters");

        RuleFor(x => x.EmployeeCount)
            .GreaterThan(0).WithMessage("Employee count must be greater than 0");
    }
}