namespace MaproSSO.Domain.Enums;

public enum InspectionStatus
{
    Pending,
    InProgress,
    Completed
}

public enum InspectionFrequency
{
    Daily,
    Weekly,
    Monthly,
    Quarterly,
    Annual
}

public enum ObservationType
{
    Safety,
    Quality,
    Environment,
    Compliance
}

public enum ObservationSeverity
{
    Low,
    Medium,
    High,
    Critical
}