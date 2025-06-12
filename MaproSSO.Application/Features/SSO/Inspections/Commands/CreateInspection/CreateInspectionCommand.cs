using System;
using MediatR;
using MaproSSO.Application.Common.Models;
using MaproSSO.Application.Common.Attributes;

namespace MaproSSO.Application.Features.SSO.Inspections.Commands.CreateInspection
{
    [Authorize(Permissions = "inspection.create")]
    public class CreateInspectionCommand : IRequest<Result<InspectionDto>>
    {
        public Guid ProgramId { get; set; }
        public Guid AreaId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public Guid InspectorUserId { get; set; }
        public DateTime ScheduledDate { get; set; }
    }
}
