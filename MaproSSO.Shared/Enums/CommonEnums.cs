using System.ComponentModel;

namespace MaproSSO.Shared.Enums;

public enum Status
{
    [Description("Activo")]
    Active = 1,

    [Description("Inactivo")]
    Inactive = 0,

    [Description("Pendiente")]
    Pending = 2,

    [Description("Suspendido")]
    Suspended = 3,

    [Description("Cancelado")]
    Cancelled = 4
}

public enum Priority
{
    [Description("Baja")]
    Low = 1,

    [Description("Media")]
    Medium = 2,

    [Description("Alta")]
    High = 3,

    [Description("Crítica")]
    Critical = 4
}

public enum Gender
{
    [Description("Masculino")]
    Male = 1,

    [Description("Femenino")]
    Female = 2,

    [Description("Otro")]
    Other = 3,

    [Description("Prefiero no decir")]
    PreferNotToSay = 4
}

public enum DataType
{
    [Description("Texto")]
    String = 1,

    [Description("Número")]
    Integer = 2,

    [Description("Decimal")]
    Decimal = 3,

    [Description("Booleano")]
    Boolean = 4,

    [Description("Fecha")]
    DateTime = 5,

    [Description("JSON")]
    Json = 6
}

public enum FileCategory
{
    [Description("Imagen")]
    Image = 1,

    [Description("Documento")]
    Document = 2,

    [Description("Video")]
    Video = 3,

    [Description("Audio")]
    Audio = 4,

    [Description("Archivo")]
    Archive = 5,

    [Description("Otro")]
    Other = 6
}

public enum NotificationChannel
{
    [Description("Sistema")]
    System = 1,

    [Description("Email")]
    Email = 2,

    [Description("SMS")]
    SMS = 3,

    [Description("Push")]
    Push = 4,

    [Description("WhatsApp")]
    WhatsApp = 5
}

public enum AuditResult
{
    [Description("Exitoso")]
    Success = 1,

    [Description("Fallido")]
    Failed = 2,

    [Description("Advertencia")]
    Warning = 3
}