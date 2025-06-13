namespace MaproSSO.Shared.Constants;

public static class SystemConstants
{
    public const string APPLICATION_NAME = "MaproSSO";
    public const string APPLICATION_VERSION = "1.0.0";
    public const string DEFAULT_CULTURE = "es-PE";
    public const string DEFAULT_TIMEZONE = "America/Lima";
    public const string DEFAULT_CURRENCY = "USD";

    public static class Roles
    {
        public const string SUPER_ADMIN = "SuperAdmin";
        public const string ADMIN_SSO = "AdminSSO";
        public const string LEADER_AREA = "LeaderArea";
        public const string USER_AREA = "UserArea";
        public const string CONTRACTOR_RESPONSIBLE = "ContractorResponsible";
    }

    public static class Permissions
    {
        // Tenant permissions
        public const string TENANT_VIEW = "tenant.view";
        public const string TENANT_CREATE = "tenant.create";
        public const string TENANT_EDIT = "tenant.edit";
        public const string TENANT_DELETE = "tenant.delete";

        // User permissions
        public const string USER_VIEW = "user.view";
        public const string USER_CREATE = "user.create";
        public const string USER_EDIT = "user.edit";
        public const string USER_DELETE = "user.delete";

        // Pillar permissions
        public const string PILLAR_VIEW = "pillar.view";
        public const string PILLAR_UPLOAD = "pillar.upload";
        public const string PILLAR_EDIT = "pillar.edit";
        public const string PILLAR_DELETE = "pillar.delete";

        // Announcement permissions
        public const string ANNOUNCEMENT_VIEW = "announcement.view";
        public const string ANNOUNCEMENT_CREATE = "announcement.create";
        public const string ANNOUNCEMENT_EDIT = "announcement.edit";
        public const string ANNOUNCEMENT_DELETE = "announcement.delete";

        // Inspection permissions
        public const string INSPECTION_VIEW = "inspection.view";
        public const string INSPECTION_CREATE = "inspection.create";
        public const string INSPECTION_EDIT = "inspection.edit";
        public const string INSPECTION_DELETE = "inspection.delete";

        // Audit permissions
        public const string AUDIT_VIEW = "audit.view";
        public const string AUDIT_CREATE = "audit.create";
        public const string AUDIT_EDIT = "audit.edit";
        public const string AUDIT_EVALUATE = "audit.evaluate";

        // Accident permissions
        public const string ACCIDENT_VIEW = "accident.view";
        public const string ACCIDENT_CREATE = "accident.create";
        public const string ACCIDENT_EDIT = "accident.edit";
        public const string ACCIDENT_INVESTIGATE = "accident.investigate";

        // Training permissions
        public const string TRAINING_VIEW = "training.view";
        public const string TRAINING_CREATE = "training.create";
        public const string TRAINING_EDIT = "training.edit";
        public const string TRAINING_ATTENDANCE = "training.attendance";

        // Subscription permissions
        public const string SUBSCRIPTION_VIEW = "subscription.view";
        public const string SUBSCRIPTION_MANAGE = "subscription.manage";
    }

    public static class ModuleCodes
    {
        public const string TENANT = "Tenant";
        public const string SUBSCRIPTION = "Subscription";
        public const string SECURITY = "Security";
        public const string PILLAR = "Pillar";
        public const string ANNOUNCEMENT = "Announcement";
        public const string INSPECTION = "Inspection";
        public const string AUDIT = "Audit";
        public const string ACCIDENT = "Accident";
        public const string TRAINING = "Training";
        public const string AUDIT_LOGGING = "AuditLogging";
    }

    public static class FileTypes
    {
        public static readonly string[] ALLOWED_IMAGE_EXTENSIONS = { ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".webp" };
        public static readonly string[] ALLOWED_DOCUMENT_EXTENSIONS = { ".pdf", ".doc", ".docx", ".xls", ".xlsx", ".ppt", ".pptx", ".txt", ".rtf" };
        public static readonly string[] ALLOWED_VIDEO_EXTENSIONS = { ".mp4", ".avi", ".mov", ".wmv", ".flv", ".webm" };

        public const long MAX_FILE_SIZE = 100 * 1024 * 1024; // 100MB
        public const long MAX_IMAGE_SIZE = 10 * 1024 * 1024; // 10MB
        public const long MAX_VIDEO_SIZE = 500 * 1024 * 1024; // 500MB
    }

    public static class ValidationRules
    {
        public const int MIN_PASSWORD_LENGTH = 8;
        public const int MAX_PASSWORD_LENGTH = 128;
        public const int PASSWORD_HISTORY_COUNT = 5;
        public const int MAX_LOGIN_ATTEMPTS = 5;
        public const int LOCKOUT_DURATION_MINUTES = 30;
        public const int SESSION_TIMEOUT_MINUTES = 60;
        public const int PASSWORD_EXPIRY_DAYS = 90;
    }

    public static class AuditActions
    {
        public const string CREATE = "Create";
        public const string UPDATE = "Update";
        public const string DELETE = "Delete";
        public const string VIEW = "View";
        public const string LOGIN = "Login";
        public const string LOGOUT = "Logout";
        public const string UPLOAD = "Upload";
        public const string DOWNLOAD = "Download";
        public const string EXPORT = "Export";
        public const string IMPORT = "Import";
    }

    public static class SubscriptionStatus
    {
        public const string TRIAL = "Trial";
        public const string ACTIVE = "Active";
        public const string SUSPENDED = "Suspended";
        public const string CANCELLED = "Cancelled";
        public const string EXPIRED = "Expired";
    }

    public static class NotificationTypes
    {
        public const string INFO = "Info";
        public const string WARNING = "Warning";
        public const string ERROR = "Error";
        public const string SUCCESS = "Success";
    }
}