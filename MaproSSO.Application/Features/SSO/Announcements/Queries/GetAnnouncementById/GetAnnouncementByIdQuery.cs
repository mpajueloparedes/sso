using System;
using MediatR;
using MaproSSO.Application.Common.Models;
using MaproSSO.Application.Common.Attributes;

namespace MaproSSO.Application.Features.SSO.Announcements.Queries.GetAnnouncementById
{
    [Authorize(Permissions = "announcement.view")]
    public class GetAnnouncementByIdQuery : IRequest<Result<AnnouncementDetailDto>>
    {
        public Guid Id { get; set; }
    }
}