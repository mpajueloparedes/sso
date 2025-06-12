using System;
using MediatR;
using MaproSSO.Application.Common.Models;
using MaproSSO.Application.Common.Attributes;

namespace MaproSSO.Application.Features.SSO.Announcements.Commands.CompleteAction
{
    [Authorize(Permissions = "announcement.edit")]
    public class CompleteActionCommand : IRequest<Result>
    {
        public Guid ActionId { get; set; }
    }
}