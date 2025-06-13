using MaproSSO.Domain.Common;
using MaproSSO.Domain.Entities.Security;
using MaproSSO.Domain.Exceptions;

namespace MaproSSO.Domain.Entities.Areas
{
    public class Area : BaseAggregateRoot, IAggregateRoot
    {
        private readonly List<AreaUser> _areaUsers = new();

        public Guid TenantId { get; private set; }
        public string AreaName { get; private set; }
        public string AreaCode { get; private set; }
        public string Description { get; private set; }
        public Guid? ParentAreaId { get; private set; }
        public Guid? ManagerUserId { get; private set; }
        public bool IsActive { get; private set; }

        public virtual Area ParentArea { get; private set; }
        public virtual User ManagerUser { get; private set; }
        public IReadOnlyCollection<AreaUser> AreaUsers => _areaUsers.AsReadOnly();

        private Area() { }

        public static Area Create(
            Guid tenantId,
            string areaName,
            string areaCode,
            string description = null,
            Guid? parentAreaId = null,
            Guid? managerUserId = null,
            Guid? createdBy = null)
        {
            if (string.IsNullOrWhiteSpace(areaName))
                throw new DomainException("El nombre del área es requerido");

            if (string.IsNullOrWhiteSpace(areaCode))
                throw new DomainException("El código del área es requerido");

            var area = new Area
            {
                TenantId = tenantId,
                AreaName = areaName,
                AreaCode = areaCode.ToUpperInvariant(),
                Description = description,
                ParentAreaId = parentAreaId,
                ManagerUserId = managerUserId,
                IsActive = true
            };

            if (createdBy.HasValue)
                area.SetCreatedInfo(createdBy.Value);

            return area;
        }

        public void Update(
            string areaName,
            string description,
            Guid? parentAreaId,
            Guid? managerUserId,
            Guid updatedBy)
        {
            AreaName = areaName;
            Description = description;
            ParentAreaId = parentAreaId;
            ManagerUserId = managerUserId;

            SetUpdatedInfo(updatedBy);
        }

        public void AssignUser(Guid userId, string role, Guid assignedBy)
        {
            if (_areaUsers.Any(au => au.UserId == userId))
                throw new BusinessRuleValidationException("El usuario ya está asignado al área");

            if (role != "Leader" && role != "User")
                throw new BusinessRuleValidationException("Rol de área inválido");

            _areaUsers.Add(new AreaUser(Id, userId, role, assignedBy));
        }

        public void RemoveUser(Guid userId)
        {
            var areaUser = _areaUsers.FirstOrDefault(au => au.UserId == userId);
            if (areaUser != null)
            {
                _areaUsers.Remove(areaUser);
            }
        }

        public void Deactivate(Guid userId)
        {
            if (!IsActive)
                throw new BusinessRuleValidationException("El área ya está inactiva");

            IsActive = false;
            SetUpdatedInfo(userId);
        }

        public void Activate(Guid userId)
        {
            if (IsActive)
                throw new BusinessRuleValidationException("El área ya está activa");

            IsActive = true;
            SetUpdatedInfo(userId);
        }

        public bool HasUser(Guid userId)
        {
            return _areaUsers.Any(au => au.UserId == userId);
        }

        public bool IsUserLeader(Guid userId)
        {
            return _areaUsers.Any(au => au.UserId == userId && au.Role == "Leader");
        }
    }
}