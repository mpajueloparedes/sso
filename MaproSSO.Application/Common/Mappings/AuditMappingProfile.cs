using AutoMapper;
using MaproSSO.Application.Features.Audit.DTOs;
using MaproSSO.Domain.Entities.Audit;
using System.Text.Json;

namespace MaproSSO.Application.Common.Mappings;

public class AuditMappingProfile : Profile
{
    public AuditMappingProfile()
    {
        CreateMap<AuditLog, AuditLogDto>()
            .ForMember(dest => dest.TenantName, opt => opt.MapFrom(src => src.Tenant != null ? src.Tenant.CompanyName : null))
            .ForMember(dest => dest.ChangedProperties, opt => opt.MapFrom(src => ParseChangedProperties(src.OldValues, src.NewValues)));

        CreateMap<AccessLog, AccessLogDto>()
            .ForMember(dest => dest.TenantName, opt => opt.MapFrom(src => src.Tenant != null ? src.Tenant.CompanyName : null));
    }

    private static Dictionary<string, object>? ParseChangedProperties(string? oldValues, string? newValues)
    {
        if (string.IsNullOrEmpty(oldValues) || string.IsNullOrEmpty(newValues))
            return null;

        try
        {
            var oldDict = JsonSerializer.Deserialize<Dictionary<string, object>>(oldValues);
            var newDict = JsonSerializer.Deserialize<Dictionary<string, object>>(newValues);

            if (oldDict == null || newDict == null)
                return null;

            var changes = new Dictionary<string, object>();

            foreach (var key in newDict.Keys)
            {
                if (!oldDict.ContainsKey(key) || !Equals(oldDict[key], newDict[key]))
                {
                    changes[key] = new { Old = oldDict.GetValueOrDefault(key), New = newDict[key] };
                }
            }

            return changes.Any() ? changes : null;
        }
        catch
        {
            return null;
        }
    }
}