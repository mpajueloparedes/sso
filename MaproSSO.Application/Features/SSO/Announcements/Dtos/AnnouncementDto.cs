using System;
using System.Collections.Generic;
using MaproSSO.Application.Common.Models;
using MaproSSO.Application.Common.Mappings;
using MaproSSO.Domain.Entities.SSO;
using MaproSSO.Domain.Enums;
using AutoMapper;
using MaproSSO.Application.Features.SSO.Announcements.Commands.CreateAnnouncement;
using MaproSSO.Domain.Entities.Announcements;

namespace MaproSSO.Application.Features.SSO.Announcements
{
    public class AnnouncementDto : AuditableDto, IMapFrom<Announcement>
    {
        public Guid AreaId { get; set; }
        public string AreaName { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public AnnouncementType Type { get; set; }
        public Severity Severity { get; set; }
        public AnnouncementStatus Status { get; set; }
        public string Location { get; set; }
        public DateTime ReportedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public List<AnnouncementImageDto> Images { get; set; }
        public int CorrectiveActionCount { get; set; }
        public int CompletedActionCount { get; set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<Announcement, AnnouncementDto>()
                .ForMember(d => d.AreaName, opt => opt.MapFrom(s => s.Area.AreaName))
                .ForMember(d => d.CorrectiveActionCount, opt => opt.MapFrom(s => s.CorrectiveActions.Count))
                .ForMember(d => d.CompletedActionCount, opt => opt.MapFrom(s =>
                    s.CorrectiveActions.Count(ca => ca.Status == ActionStatus.Completed)));
        }
    }

    public class AnnouncementListDto : AnnouncementDto
    {
        public string CreatedByName { get; set; }
        public decimal CompletionPercentage { get; set; }
        public bool HasOverdueActions { get; set; }
    }

    public class AnnouncementDetailDto : AnnouncementDto
    {
        public string CreatedByName { get; set; }
        public decimal CompletionPercentage { get; set; }
        public List<CorrectiveActionDto> CorrectiveActions { get; set; }
    }

    public class CorrectiveActionDto : AuditableDto, IMapFrom<CorrectiveAction>
    {
        public Guid AnnouncementId { get; set; }
        public string Description { get; set; }
        public Guid ResponsibleUserId { get; set; }
        public string ResponsibleUserName { get; set; }
        public DateTime DueDate { get; set; }
        public ActionStatus Status { get; set; }
        public DateTime? CompletedAt { get; set; }
        public bool IsOverdue { get; set; }
        public int DaysUntilDue { get; set; }
        public List<EvidenceDto> Evidences { get; set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<CorrectiveAction, CorrectiveActionDto>()
                .ForMember(d => d.ResponsibleUserName, opt => opt.MapFrom(s => s.ResponsibleUser.FullName))
                .ForMember(d => d.IsOverdue, opt => opt.MapFrom(s => s.IsOverdue))
                .ForMember(d => d.DaysUntilDue, opt => opt.MapFrom(s => s.DaysUntilDue));
        }
    }

    public class EvidenceDto : IMapFrom<ActionEvidence>
    {
        public Guid Id { get; set; }
        public string Description { get; set; }
        public string EvidenceUrl { get; set; }
        public DateTime UploadedAt { get; set; }
        public string UploadedByName { get; set; }
    }

    public class AnnouncementReportDto
    {
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }

        // Totales
        public int TotalAnnouncements { get; set; }
        public int PendingCount { get; set; }
        public int InProgressCount { get; set; }
        public int CompletedCount { get; set; }

        // Por severidad
        public int LowSeverityCount { get; set; }
        public int MediumSeverityCount { get; set; }
        public int HighSeverityCount { get; set; }
        public int CriticalSeverityCount { get; set; }

        // Por tipo
        public int SafetyCount { get; set; }
        public int QualityCount { get; set; }
        public int EnvironmentCount { get; set; }
        public int HealthCount { get; set; }

        // Acciones correctivas
        public int TotalCorrectiveActions { get; set; }
        public int CompletedActions { get; set; }
        public int OverdueActions { get; set; }

        // Métricas
        public double AverageResolutionDays { get; set; }
        public decimal CompletionRate { get; set; }
        public decimal ActionCompletionRate { get; set; }

        // Por área
        public List<AnnouncementByAreaDto> ByArea { get; set; }

        // Tendencia
        public List<MonthlyTrendDto> MonthlyTrend { get; set; }
    }

    public class AnnouncementByAreaDto
    {
        public Guid AreaId { get; set; }
        public string AreaName { get; set; }
        public int Count { get; set; }
        public int CompletedCount { get; set; }
        public double AverageResolutionDays { get; set; }
    }

    public class MonthlyTrendDto
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public int Count { get; set; }
        public int HighSeverityCount { get; set; }
    }
}