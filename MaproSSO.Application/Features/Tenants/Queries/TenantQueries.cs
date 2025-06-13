using MediatR;
using MaproSSO.Application.Features.Tenants.DTOs;

namespace MaproSSO.Application.Features.Tenants.Queries;

public record GetTenantsQuery : IRequest<List<TenantListDto>>
{
    public bool? IsActive { get; init; }
    public string? Industry { get; init; }
    public string? Country { get; init; }
    public string? SearchTerm { get; init; }
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 10;
    public string SortBy { get; init; } = "CompanyName";
    public bool SortDescending { get; init; } = false;
}

public record GetTenantByIdQuery : IRequest<TenantDto?>
{
    public Guid TenantId { get; init; }
    public bool IncludeSettings { get; init; } = false;
    public bool IncludeSubscription { get; init; } = false;
    public bool IncludeStatistics { get; init; } = false;
}

public record GetTenantByEmailQuery : IRequest<TenantDto?>
{
    public string Email { get; init; } = string.Empty;
}

public record GetTenantByTaxIdQuery : IRequest<TenantDto?>
{
    public string TaxId { get; init; } = string.Empty;
}

public record GetTenantDashboardQuery : IRequest<TenantDashboardDto>
{
    public Guid TenantId { get; init; }
}

public record GetTenantStatisticsQuery : IRequest<TenantStatisticsDto>
{
    public Guid TenantId { get; init; }
    public DateTime? FromDate { get; init; }
    public DateTime? ToDate { get; init; }
}

public record GetTenantsForSuperAdminQuery : IRequest<List<TenantListDto>>
{
    public string? SearchTerm { get; init; }
    public bool? IsActive { get; init; }
    public DateTime? CreatedAfter { get; init; }
    public DateTime? CreatedBefore { get; init; }
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 20;
}