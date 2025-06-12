using AutoMapper;
using MaproSSO.Application.Features.Areas.DTOs;
using MaproSSO.Application.Features.Announcements.DTOs;
using MaproSSO.Application.Features.Pillars.DTOs;
using MaproSSO.Domain.Entities.Areas;
using MaproSSO.Domain.Entities.Announcements;
using MaproSSO.Domain.Entities.Pillars;
using MaproSSO.Application.Features.SSO.Announcements.Commands.CreateAnnouncement;
using MaproSSO.Application.Features.SSO.Announcements;
using MaproSSO.Domain.Entities.SSO;

namespace MaproSSO.Application.Common.Mappings;

public class SSOMappingProfile : Profile
{
    public SSOMappingProfile()
    {
        // Areas mapping
        CreateMap<Area, AreaDto>()
            .ForMember(dest => dest.ParentAreaName, opt => opt.MapFrom(src => src.ParentArea != null ? src.ParentArea.AreaName : null))
            .ForMember(dest => dest.ManagerName, opt => opt.MapFrom(src => src.Manager != null ? $"{src.Manager.FirstName} {src.Manager.LastName}" : null));

        CreateMap<AreaUser, AreaUserDto>()
            .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => $"{src.User.FirstName} {src.User.LastName}"))
            .ForMember(dest => dest.UserEmail, opt => opt.MapFrom(src => src.User.Email));

        CreateMap<ContractorCompany, ContractorCompanyDto>();

        // Announcements mapping
        CreateMap<Announcement, AnnouncementDto>()
            .ForMember(dest => dest.AreaName, opt => opt.MapFrom(src => src.Area.AreaName));

        CreateMap<AnnouncementImage, AnnouncementImageDto>()
            .ForMember(dest => dest.UploadedByName, opt => opt.MapFrom(src => $"{src.UploadedByUser.FirstName} {src.UploadedByUser.LastName}"));

        CreateMap<CorrectiveAction, CorrectiveActionDto>()
            .ForMember(dest => dest.ResponsibleUserName, opt => opt.MapFrom(src => $"{src.ResponsibleUser.FirstName} {src.ResponsibleUser.LastName}"));

        CreateMap<ActionEvidence, ActionEvidenceDto>()
            .ForMember(dest => dest.UploadedByName, opt => opt.MapFrom(src => $"{src.UploadedByUser.FirstName} {src.UploadedByUser.LastName}"));

        // Pillars mapping
        CreateMap<Pillar, PillarDto>();

        CreateMap<DocumentFolder, DocumentFolderDto>()
            .ForMember(dest => dest.PillarName, opt => opt.MapFrom(src => src.Pillar.PillarName))
            .ForMember(dest => dest.AreaName, opt => opt.MapFrom(src => src.Area.AreaName));

        CreateMap<Document, DocumentDto>()
            .ForMember(dest => dest.FolderName, opt => opt.MapFrom(src => src.Folder.FolderName))
            .ForMember(dest => dest.CreatedByName, opt => opt.MapFrom(src => $"{src.CreatedByUser.FirstName} {src.CreatedByUser.LastName}"));
    }
}