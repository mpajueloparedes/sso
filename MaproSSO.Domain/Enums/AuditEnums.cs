namespace MaproSSO.Domain.Enums;

public enum AuditAction
{
    Create,
    Update,
    Delete,
    View,
    Login,
    Logout,
    PasswordChange,
    PermissionChange,
    Export,
    Import,
    Download,
    Upload
}

public enum AccessAction
{
    Login,
    Logout,
    FailedLogin,
    PasswordChange,
    AccountLocked,
    AccountUnlocked,
    TwoFactorEnabled,
    TwoFactorDisabled,
    SessionExpired
}