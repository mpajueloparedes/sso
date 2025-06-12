using System;
using System.IO;
using MediatR;
using MaproSSO.Application.Common.Models;
using MaproSSO.Application.Common.Attributes;

namespace MaproSSO.Application.Features.SSO.Inspections.Commands.StartInspection
{
    [Authorize(Permissions = "inspection.edit")]
    public class StartInspectionCommand : IRequest<r>
    {
        public Guid InspectionId { get; set; }
        public Stream DocumentStream { get; set; }
        public string FileName { get; set; }
        public string ContentType { get; set; }
    }
}