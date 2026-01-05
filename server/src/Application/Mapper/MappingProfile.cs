using AutoMapper;
using Domain.Entities;
using Domain.Models;

namespace Application.Mapping;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Topic, TopicDto>()
                    .ForMember(dest => dest.AuthorFullName, opt => opt.MapFrom(src => $"{src.User.Name} {src.User.Surname}"))
                    .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.User.UserName))
                    .ForMember(dest => dest.ForumTitle, opt => opt.MapFrom(src => src.Forum.Title))
                    .ForMember(dest => dest.CommentNum, opt => opt.MapFrom(src => src.CommentNum));

        CreateMap<Forum, ForumDto>()
            .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.User.UserName))
            .ForMember(dest => dest.TopicNum, opt => opt.MapFrom(src => src.TopicNum));


        CreateMap<CreateForumDto, Forum>();

        CreateMap<CreateTopicDto, Topic>();

        CreateMap<CreateCommentDto, Comment>();

        CreateMap<Comment, CommentDto>()
            .ForMember(dest => dest.AuthorFullName, opt => opt.MapFrom(src => $"{src.User.Name} {src.User.Surname}"))
            .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.User.UserName));

        CreateMap<User, UserDto>()
            .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => $"{src.Name} {src.Surname}"));
        CreateMap<User, AuthorizedUserDto>().ReverseMap();
    }

}

