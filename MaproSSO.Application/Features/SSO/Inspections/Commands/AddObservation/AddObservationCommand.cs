//using System;
//using System.Collections.Generic;
//using MediatR;
//using MaproSSO.Application.Common.Models;
//using MaproSSO.Application.Common.Attributes;
//using MaproSSO.Domain.Enums;

//namespace MaproSSO.Application.Features.SSO.Inspections.Commands.AddObservation
//{
//    [Authorize(Permissions = "inspection.edit")]
//    public class AddObservationCommand : IRequest<Result<ObservationDto>>
//    {
//        public Guid InspectionId { get; set; }
//        public string Description { get; set; }
//        public string Type { get; set; } // Safety, Quality, Environment, Compliance
//        public Severity Severity { get; set; }
//        public Guid ResponsibleUserId { get; set; }
//        public DateTime DueDate {