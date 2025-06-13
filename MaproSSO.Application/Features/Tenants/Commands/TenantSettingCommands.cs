using MediatR;
using MaproSSO.Application.Features.Tenants.DTOs;

namespace MaproSSO.Application.Features.Tenants.Commands;

public record CreateTenantSettingCommand : IRequest<TenantSettingDto>
{
    public Guid TenantId { get; init; }
    public string SettingKey { get; init; } = string.Empty;
    public string SettingValue { get; init; } = string.Empty;
    public string DataType { get; init; } = "String";
    public string? Description { get; init; }
    public bool IsEncrypted { get; init; } = false;
}

public record UpdateTenantSettingCommand : IRequest<TenantSettingDto>
{
    public Guid SettingId { get; init; }
    public string SettingValue { get; init; } = string.Empty;
    public string? Description { get; init; }
    public bool IsEncrypted { get; init; } = false;
}

public record DeleteTenantSettingCommand : IRequest<bool>
{
    public Guid SettingId { get; init; }
}

public record BulkUpdateTenantSettingsCommand : IRequest<List<TenantSettingDto>>
{
    public Guid TenantId { get; init; }
    public Dictionary<string, object> Settings { get; init; } = new();
}

public record ResetTenantSettingsCommand : IRequest<List<TenantSettingDto>>
{
    public Guid TenantId { get; init; }
    public List<string> SettingKeys { get; init; } = new();
}