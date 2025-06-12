using System;

namespace MaproSSO.Application.Common.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class CacheAttribute : Attribute
    {
        public int DurationInSeconds { get; set; }
        public string CacheKeyPrefix { get; set; }
        public bool VaryByTenant { get; set; } = true;
        public bool VaryByUser { get; set; } = false;
    }
}