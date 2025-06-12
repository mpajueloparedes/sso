using AutoMapper;
using MaproSSO.Application.Features.Inspections.DTOs;
using MaproSSO.Domain.Entities.Inspections;

namespace MaproSSO.Application.Common.Mappings;

public class InspectionMappingProfile : Profile
{
    public InspectionMappingProfile()
    {
        CreateMap<Inspection, InspectionDto>()
            .ForMember(dest => dest.ProgramName, opt => opt.MapFrom(src => src.Program.ProgramName))
            .ForMember(dest => dest.AreaName, opt => opt.MapFrom(src => src.Area.AreaName))
            .ForMember(dest => dest.InspectorName, opt => opt.MapFrom(src => $"{src.Inspector.FirstName} {src.Inspector.LastName}"));

        CreateMap<InspectionProgram, InspectionProgramDto>();

        CreateMap<InspectionProgramDetail, InspectionProgramDetailDto>()
            .ForMember(dest => dest.AreaName, opt => opt.MapFrom(src => src.Area.AreaName));

        CreateMap<InspectionObservation, InspectionObservationDto>()
            .ForMember(dest => dest.ResponsibleUserName, opt => opt.MapFrom(src => $"{src.ResponsibleUser.FirstName} {src.ResponsibleUser.LastName}"));

        CreateMap<ObservationImage, ObservationImageDto>();

        CreateMap<ObservationEvidence, ObservationEvidenceDto>()
            .ForMember(dest => dest.UploadedByName, opt => opt.MapFrom(src => $"{src.UploadedByUser.FirstName} {src.UploadedByUser.LastName}"));
    }
}