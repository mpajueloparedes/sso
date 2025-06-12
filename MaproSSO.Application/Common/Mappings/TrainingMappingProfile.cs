using AutoMapper;
using MaproSSO.Application.Features.Trainings.DTOs;
using MaproSSO.Domain.Entities.Trainings;

namespace MaproSSO.Application.Common.Mappings;

public class TrainingMappingProfile : Profile
{
    public TrainingMappingProfile()
    {
        CreateMap<Training, TrainingDto>()
            .ForMember(dest => dest.AreaName, opt => opt.MapFrom(src => src.Area.AreaName))
            .ForMember(dest => dest.InstructorName, opt => opt.MapFrom(src => src.Instructor != null ? $"{src.Instructor.FirstName} {src.Instructor.LastName}" : src.ExternalInstructor))
            .ForMember(dest => dest.CurrentParticipants, opt => opt.MapFrom(src => src.Participants.Count));

        CreateMap<TrainingParticipant, TrainingParticipantDto>()
            .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => $"{src.User.FirstName} {src.User.LastName}"))
            .ForMember(dest => dest.UserEmail, opt => opt.MapFrom(src => src.User.Email));
    }
}