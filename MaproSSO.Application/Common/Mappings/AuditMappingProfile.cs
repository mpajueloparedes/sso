using AutoMapper;
using MaproSSO.Application.Features.Audits.DTOs;
using MaproSSO.Domain.Entities.Audits;

namespace MaproSSO.Application.Common.Mappings;

public class AuditMappingProfile : Profile
{
    public AuditMappingProfile()
    {
        CreateMap<Audit, AuditDto>()
            .ForMember(dest => dest.ProgramName, opt => opt.MapFrom(src => src.Program.ProgramName))
            .ForMember(dest => dest.AreaName, opt => opt.MapFrom(src => src.Area.AreaName))
            .ForMember(dest => dest.AuditorName, opt => opt.MapFrom(src => $"{src.Auditor.FirstName} {src.Auditor.LastName}"));

        CreateMap<AuditProgram, AuditProgramDto>();

        CreateMap<AuditCategory, AuditCategoryDto>();

        CreateMap<AuditCriteria, AuditCriteriaDto>();

        CreateMap<AuditEvaluation, AuditEvaluationDto>()
            .ForMember(dest => dest.CriteriaDescription, opt => opt.MapFrom(src => src.Criteria.Description))
            .ForMember(dest => dest.MaxScore, opt => opt.MapFrom(src => src.Criteria.MaxScore))
            .ForMember(dest => dest.EvaluatedByName, opt => opt.MapFrom(src => $"{src.EvaluatedByUser.FirstName} {src.EvaluatedByUser.LastName}"));

        CreateMap<AuditEvidence, AuditEvidenceDto>()
            .ForMember(dest => dest.UploadedByName, opt => opt.MapFrom(src => $"{src.UploadedByUser.FirstName} {src.UploadedByUser.LastName}"));
    }
}