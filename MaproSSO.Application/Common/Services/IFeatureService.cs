using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MaproSSO.Application.Common.Services
{
    public interface IFeatureService
    {
        Task<bool> HasAccessToFeatureAsync(Guid tenantId, string featureCode);
        Task<Dictionary<string, bool>> GetFeaturesStatusAsync(Guid tenantId);
        Task<FeatureLimitInfo> GetFeatureLimitAsync(Guid tenantId, string featureCode);
        Task UpdateFeatureUsageAsync(Guid tenantId, string featureCode, int newUsage);
        Task ResetPeriodicUsagesAsync();
    }

    public class FeatureLimitInfo
    {
        public string FeatureCode { get; set; }
        public string FeatureName { get; set; }
        public int CurrentUsage { get; set; }
        public int? Limit { get; set; }
        public string ResetPeriod { get; set; }
        public DateTime? NextResetDate { get; set; }
        public bool IsAvailable { get; set; }
        public string Message { get; set; }
    }
}