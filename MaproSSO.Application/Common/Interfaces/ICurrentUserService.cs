using System;
using System.Collections.Generic;

namespace MaproSSO.Application.Common.Interfaces
{
    public interface ICurrentUserService
    {
        Guid? UserId { get; }
        Guid? TenantId { get; }
        string UserName { get; }
        string Email { get; }
        bool IsAuthenticated { get; }
        bool IsSuperAdmin { get; }
        List<string> Roles { get; }
        List<string> Permissions { get; }

        bool HasRole(string roleName);
        bool HasPermission(string permissionCode);
        bool HasAnyRole(params string[] roleNames);
        bool HasAnyPermission(params string[] permissionCodes);
        bool HasAllPermissions(params string[] permissionCodes);
    }
}