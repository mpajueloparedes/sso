namespace MaproSSO.Application.Features.Tenants.DTOs;

public class TenantSettingDto
{
    public Guid SettingId { get; set; }
    public Guid TenantId { get; set; }
    public string SettingKey { get; set; } = string.Empty;
    public string SettingValue { get; set; } = string.Empty;
    public string DataType { get; set; } = string.Empty; // String, Int, Bool, Json
    public string? Description { get; set; }
    public bool IsEncrypted { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    // Helper properties for typed access
    public string StringValue => DataType == "String" ? SettingValue : string.Empty;
    public int IntValue => DataType == "Int" && int.TryParse(SettingValue, out var intVal) ? intVal : 0;
    public bool BoolValue => DataType == "Bool" && bool.TryParse(SettingValue, out var boolVal) ? boolVal : false;
    public T? JsonValue<T>() where T : class => DataType == "Json" ? System.Text.Json.JsonSerializer.Deserialize<T>(SettingValue) : null;
}

public class TenantSettingGroupDto
{
    public string GroupName { get; set; } = string.Empty;
    public string GroupDescription { get; set; } = string.Empty;
    public List<TenantSettingDto> Settings { get; set; } = new();
}

public class TenantSettingsCollectionDto
{
    public Guid TenantId { get; set; }
    public string TenantName { get; set; } = string.Empty;
    public List<TenantSettingGroupDto> SettingGroups { get; set; } = new();
    public Dictionary<string, object> FlattenedSettings { get; set; } = new();
}