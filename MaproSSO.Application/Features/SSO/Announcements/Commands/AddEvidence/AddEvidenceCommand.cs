using System;
using System.IO;
using MediatR;
using MaproSSO.Application.Common.Models;
using MaproSSO.Application.Common.Attributes;

namespace MaproSSO.Application.Features.SSO.Announcements.Commands.AddEvidence
{
    [Authorize(Permissions = "announcement.edit")]
    public class AddEvidenceCommand : IRequest<Result<EvidenceDto>>
    {
        public Guid ActionId { get; set; }
        public string Description { get; set; }
        public Stream FileStream { get; set; }
        public string FileName { get; set; }
        public string ContentType { get; set; }
    }
}