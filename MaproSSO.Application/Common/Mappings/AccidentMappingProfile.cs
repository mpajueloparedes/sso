using AutoMapper;
using MaproSSO.Application.Features.Accidents.Commands;
using MaproSSO.Application.Features.Accidents.DTOs;
using MaproSSO.Domain.Entities.Accidents;

namespace MaproSSO.Application.Common.Mappings;

public class AccidentMappingProfile : Profile
{
    public AccidentMappingProfile()
    {
        CreateMap<Accident, AccidentDto>();

        CreateMap<AccidentPerson, AccidentPersonDto>()
            .ForMember(dest => dest.AreaName, opt => opt.MapFrom(src => src.Area != null ? src.Area.AreaName : null));

        CreateMap<AccidentImage, AccidentImageDto>();

        CreateMap<CreateAccidentPersonDto, AccidentPerson>();
    }
}