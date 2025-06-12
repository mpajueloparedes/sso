using System;
using MediatR;
using MaproSSO.Application.Common.Models;
using MaproSSO.Application.Common.Attributes;
using MaproSSO.Domain.Enums;

namespace MaproSSO.Application.Features.SSO.Announcements.Commands.UpdateAnnouncement
{
    [Authorize(Permissions = "announcement.edit")]
    public class UpdateAnnouncementCommand : IRequest<Result>
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public AnnouncementType Type { get; set; }
        public Severity Severity { get; set; }
        public string Location { get; set; }
    }
}