using MediatR;
using MaproSSO.Application.Features.Tenants.DTOs;

namespace MaproSSO.Application.Features.Tenants.Commands;

public record CreateTenantCommand : IRequest<TenantDto>
{
    public string CompanyName { get; init; } = string.Empty;
    public string? TradeName { get; init; }
    public string TaxId { get; init; } = string.Empty;
    public string Industry { get; init; } = string.Empty;
    public int EmployeeCount { get; init; }
    public string Country { get; init; } = string.Empty;
    public string State { get; init; } = string.Empty;
    public string City { get; init; } = string.Empty;
    public string Address { get; init; } = string.Empty;
    public string? PostalCode { get; init; }
    public string Phone { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string? Website { get; init; }
    public string? LogoUrl { get; init; }
    public string TimeZone { get; init; } = "America/Lima";
    public string Culture { get; init; } = "es-PE";

    // Admin user creation
    public string AdminFirstName { get; init; } = string.Empty;
    public string AdminLastName { get; init; } = string.Empty;
    public string AdminEmail { get; init; } = string.Empty;
    public string AdminUsername { get; init; } = string.Empty;
    public string AdminPassword { get; init; } = string.Empty;

    // Subscription plan
    public Guid? PlanId { get; init; }
}

public record UpdateTenantCommand : IRequest<TenantDto>
{
    public Guid TenantId { get; init; }
    public string CompanyName { get; init; } = string.Empty;
    public string? TradeName { get; init; }
    public string Industry { get; init; } = string.Empty;
    public int EmployeeCount { get; init; }
    public string Country { get; init; } = string.Empty;
    public string State { get; init; } = string.Empty;
    public string City { get; init; } = string.Empty;
    public string Address { get; init; } = string.Empty;
    public string? PostalCode { get; init; }
    public string Phone { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string? Website { get; init; }
    public string? LogoUrl { get; init; }
    public string TimeZone { get; init; } = "America/Lima";
    public string Culture { get; init; } = "es-PE";
}

public record DeleteTenantCommand : IRequest<bool>
{
    public Guid TenantId { get; init; }
    public bool PermanentDelete { get; init; } = false;
    public string Reason { get; init; } = string.Empty;
}

public record ActivateTenantCommand : IRequest<TenantDto>
{
    public Guid TenantId { get; init; }
}

public record DeactivateTenantCommand : IRequest<TenantDto>
{
    public Guid TenantId { get; init; }
    public string Reason { get; init; } = string.Empty;
}

public record UpdateTenantLogoCommand : IRequest<TenantDto>
{
    public Guid TenantId { get; init; }
    public string LogoUrl { get; init; } = string.Empty;
}