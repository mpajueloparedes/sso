using System;
using System.Threading.Tasks;

namespace MaproSSO.Application.Common.Interfaces
{
    public interface ITenantService
    {
        Task<bool> SetCurrentTenant(Guid tenantId);
        Task<TenantInfo> GetCurrentTenant();
        Task<bool> ValidateTenant(Guid tenantId);
    }

    public class TenantInfo
    {
        public Guid TenantId { get; set; }
        public string CompanyName { get; set; }
        public string TimeZone { get; set; }
        public string Culture { get; set; }
        public string LogoUrl { get; set; }
    }
}