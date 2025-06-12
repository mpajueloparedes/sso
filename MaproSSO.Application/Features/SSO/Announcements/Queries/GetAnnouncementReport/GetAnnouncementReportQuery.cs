using System;
using MediatR;
using MaproSSO.Application.Common.Models;
using MaproSSO.Application.Common.Attributes;

namespace MaproSSO.Application.Features.SSO.Announcements.Queries.GetAnnouncementReport
{
    [Authorize(Permissions = "announcement.view")]
    public class GetAnnouncementReportQuery : IRequest<Result<AnnouncementReportDto>>
    {
        public Guid? AreaId { get; set; }
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
    }
}
