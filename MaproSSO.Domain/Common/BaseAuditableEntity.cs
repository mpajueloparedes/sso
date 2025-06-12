namespace MaproSSO.Domain.Common
{
    public abstract class BaseAuditableEntity : BaseEntity
    {
        public DateTime CreatedAt { get; protected set; }
        public Guid CreatedBy { get; protected set; }
        public DateTime? UpdatedAt { get; protected set; }
        public Guid? UpdatedBy { get; protected set; }
        public DateTime? DeletedAt { get; protected set; }
        public Guid? DeletedBy { get; protected set; }
        public bool IsDeleted => DeletedAt.HasValue;

        protected BaseAuditableEntity() : base()
        {
            CreatedAt = DateTime.UtcNow;
        }

        protected BaseAuditableEntity(Guid id) : base(id)
        {
            CreatedAt = DateTime.UtcNow;
        }

        public virtual void SetCreatedInfo(Guid userId)
        {
            CreatedBy = userId;
            CreatedAt = DateTime.UtcNow;
        }

        public virtual void SetUpdatedInfo(Guid userId)
        {
            UpdatedBy = userId;
            UpdatedAt = DateTime.UtcNow;
        }

        public virtual void SetDeletedInfo(Guid userId)
        {
            DeletedBy = userId;
            DeletedAt = DateTime.UtcNow;
        }

        public virtual void Restore()
        {
            DeletedBy = null;
            DeletedAt = null;
        }
    }
}