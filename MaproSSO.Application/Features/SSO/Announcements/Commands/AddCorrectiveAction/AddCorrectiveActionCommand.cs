using System;
using MediatR;
using MaproSSO.Application.Common.Models;
using MaproSSO.Application.Common.Attributes;

namespace MaproSSO.Application.Features.SSO.Announcements.Commands.AddCorrectiveAction
{
    [Authorize(Permissions = "announcement.edit")]
    public class AddCorrectiveActionCommand : IRequest<Result<CorrectiveActionDto>>
    {
        public Guid AnnouncementId { get; set; }
        public string Description { get; set; }
        public Guid ResponsibleUserId { get; set; }
        public DateTime DueDate { get; set; }
    }
}