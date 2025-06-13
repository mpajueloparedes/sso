using AutoMapper;
using MaproSSO.Application.Features.Pillars.DTOs;
using MaproSSO.Application.Common.Extensions;
using MaproSSO.Domain.Entities.Pillars;
using MaproSSO.Application.Features.Pillars.Queries;

namespace MaproSSO.Application.Common.Mappings;

public class PillarMappingProfile : Profile
{
    public PillarMappingProfile()
    {
        CreateMap<Pillar, PillarDto>()
            .ForMember(dest => dest.TotalFolders, opt => opt.Ignore())
            .ForMember(dest => dest.TotalDocuments, opt => opt.Ignore())
            .ForMember(dest => dest.RootFolders, opt => opt.Ignore());

        CreateMap<DocumentFolder, DocumentFolderDto>()
            .ForMember(dest => dest.PillarName, opt => opt.MapFrom(src => src.Pillar.PillarName))
            .ForMember(dest => dest.AreaName, opt => opt.MapFrom(src => src.Area.AreaName))
            .ForMember(dest => dest.ParentFolderName, opt => opt.MapFrom(src => src.ParentFolder != null ? src.ParentFolder.FolderName : null))
            .ForMember(dest => dest.DocumentCount, opt => opt.MapFrom(src => src.Documents.Count(d => d.IsCurrentVersion && d.DeletedAt == null)))
            .ForMember(dest => dest.SubFolderCount, opt => opt.MapFrom(src => src.SubFolders.Count));

        CreateMap<DocumentFolder, FolderTreeDto>()
            .ForMember(dest => dest.Level, opt => opt.Ignore())
            .ForMember(dest => dest.DocumentCount, opt => opt.MapFrom(src => src.Documents.Count(d => d.IsCurrentVersion && d.DeletedAt == null)))
            .ForMember(dest => dest.Children, opt => opt.MapFrom(src => src.SubFolders));

        CreateMap<Document, DocumentDto>()
            .ForMember(dest => dest.FolderName, opt => opt.MapFrom(src => src.Folder.FolderName))
            .ForMember(dest => dest.FileSizeFormatted, opt => opt.MapFrom(src => src.FileSizeBytes.FormatFileSize()))
            .ForMember(dest => dest.TagList, opt => opt.MapFrom(src => !string.IsNullOrEmpty(src.Tags) ?
                src.Tags.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(t => t.Trim()).ToList() :
                new List<string>()))
            .ForMember(dest => dest.CreatedByName, opt => opt.MapFrom(src => $"{src.CreatedByUser.FirstName} {src.CreatedByUser.LastName}"))
            .ForMember(dest => dest.UpdatedByName, opt => opt.MapFrom(src => src.UpdatedByUser != null ? $"{src.UpdatedByUser.FirstName} {src.UpdatedByUser.LastName}" : null))
            .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(src => src.DeletedAt.HasValue))
            .ForMember(dest => dest.Versions, opt => opt.Ignore());

        CreateMap<Document, DocumentVersionDto>()
            .ForMember(dest => dest.FileSizeFormatted, opt => opt.MapFrom(src => src.FileSizeBytes.FormatFileSize()))
            .ForMember(dest => dest.CreatedByName, opt => opt.MapFrom(src => $"{src.CreatedByUser.FirstName} {src.CreatedByUser.LastName}"));

        CreateMap<Document, TopDocumentDto>()
            .ForMember(dest => dest.PillarName, opt => opt.MapFrom(src => src.Folder.Pillar.PillarName))
            .ForMember(dest => dest.FolderPath, opt => opt.MapFrom(src => src.Folder.Path))
            .ForMember(dest => dest.FileSizeFormatted, opt => opt.MapFrom(src => src.FileSizeBytes.FormatFileSize()))
            .ForMember(dest => dest.LastAccessed, opt => opt.MapFrom(src => src.UpdatedAt ?? src.CreatedAt))
            .ForMember(dest => dest.AccessCount, opt => opt.Ignore());
    }
}