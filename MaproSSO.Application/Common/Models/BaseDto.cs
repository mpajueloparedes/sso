using System;

namespace MaproSSO.Application.Common.Models
{
    public abstract class BaseDto
    {
        public Guid Id { get; set; }
    }

    public abstract class AuditableDto : BaseDto
    {
        public DateTime CreatedAt { get; set; }
        public Guid CreatedBy { get; set; }
        public string CreatedByName { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public Guid? UpdatedBy { get; set; }
        public string UpdatedByName { get; set; }
    }
}