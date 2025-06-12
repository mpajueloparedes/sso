using System;
using MediatR;
using MaproSSO.Application.Common.Models;
using MaproSSO.Application.Common.Attributes;
using MaproSSO.Domain.Enums;

namespace MaproSSO.Application.Features.SSO.Announcements.Queries.GetAnnouncements
{
    [Authorize(Permissions = "announcement.view")]
    [Cache(DurationInSeconds = 60, VaryByTenant = true)]
    public class GetAnnouncementsQuery : IRequest<Result<PaginatedList<AnnouncementListDto>>>
    {
        public Guid? AreaId { get; set; }
        public AnnouncementStatus? Status { get; set; }
        public Severity? Severity { get; set; }
        public AnnouncementType? Type { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public string SearchTerm { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string OrderBy { get; set; } = "CreatedAt";
        public bool OrderDescending { get; set; } = true;
    }
}