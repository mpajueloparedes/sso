using System;
using System.Collections.Generic;
using MediatR;
using MaproSSO.Application.Common.Models;
using MaproSSO.Application.Common.Attributes;
using MaproSSO.Domain.Enums;

namespace MaproSSO.Application.Features.SSO.Announcements.Commands.CreateAnnouncement
{
    [Authorize(Permissions = "announcement.create")]
    public class CreateAnnouncementCommand : IRequest<Result<AnnouncementDto>>
    {
        public Guid AreaId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public AnnouncementType Type { get; set; }
        public Severity Severity { get; set; }
        public string Location { get; set; }
        public List<AnnouncementImageDto> Images { get; set; } = new();
    }

    public class AnnouncementImageDto
    {
        public string ImageUrl { get; set; }
        public string Description { get; set; }
    }
}