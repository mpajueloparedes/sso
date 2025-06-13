using MediatR;
using MaproSSO.Application.Features.Tenants.DTOs;

namespace MaproSSO.Application.Features.Tenants.Queries;

public record GetTenantSettingsQuery : IRequest<List<TenantSettingDto>>
{
    public Guid TenantId { get; init; }
    public string? SettingKey { get; init; }
    public string? GroupFilter { get; init; }
}

public record GetTenantSettingByKeyQuery : IRequest<TenantSettingDto?>
{
    public Guid TenantId { get; init; }
    public string SettingKey { get; init; } = string.Empty;
}

public record GetTenantSettingsGroupedQuery : IRequest<TenantSettingsCollectionDto>
{
    public Guid TenantId { get; init; }
}

public record GetTenantSettingValueQuery<T> : IRequest<T?>
{
    public Guid TenantId { get; init; }
    public string SettingKey { get; init; } = string.Empty;
    public T? DefaultValue { get; init; }
}